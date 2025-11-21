using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EvolveThisMatch.Lobby
{
    public class IdleManager : MonoBehaviour
    {
        private EnemyRecordSystem _enemyRecordSystem;
        private LobbyWaveSystem _waveSystem;
        private UIIdleCanvas _idleCanvas;
        private bool _isRunning = true;
        private bool _isInitialize = false;

        private void Start()
        {
            // 비로그인 상태라면 기록하지 않는다.
            if (!PlayFabAuthService.IsLoginState)
            {
                gameObject.SetActive(false);
                return;
            }

            _enemyRecordSystem = BattleManager.Instance.GetSubSystem<EnemyRecordSystem>();
            _waveSystem = BattleManager.Instance.GetSubSystem<LobbyWaveSystem>();
            _idleCanvas = GetComponentInChildren<UIIdleCanvas>();
            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;

            RequestOfflineIdleReward().Forget();
            SendDataTimer().Forget();
        }

        private void OnDestroy()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
        }

        #region 서버에 검증 요청을 보낼 조건
        /// <summary>
        /// 5분마다, 서버에 검증을 요청하여 획득
        /// </summary>
        private async UniTaskVoid SendDataTimer()
        {
            while (_isRunning)
            {
                await UniTask.Delay(TimeSpan.FromMinutes(5), ignoreTimeScale: true);

                await RequestOnlineIdleReward();
            }
        }

        /// <summary>
        /// 전투 시작 시, 서버에 검증을 요청하여 획득
        /// </summary>
        public void OnBattleInitialize()
        {
            if (!_isInitialize)
            {
                _isInitialize = true;
                return;
            }
            RequestOnlineIdleReward().Forget();
        }

        /// <summary>
        /// 게임 종료 시, 서버에 검증을 요청하여 획득
        /// </summary>
        private void OnApplicationQuit()
        {
            RequestOnlineIdleReward().Forget();

            _isRunning = false;
        }
        #endregion

        #region 서버에 방치 보상을 요청
        /// <summary>
        /// 오프라인 보상 획득 시도
        /// </summary>
        private async UniTask RequestOfflineIdleReward()
        {
            var records = _enemyRecordSystem.GetRecords();
            var tcs = new UniTaskCompletionSource();

            await UniTask.WaitUntil(() => _waveSystem.currentWave != null);

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "RequestOfflineIdleReward",
                FunctionParameter = new { CurrentWave = _waveSystem.currentWave.id, EnemyRecords = records },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        _enemyRecordSystem.ClearRecords();

                        int offlineGold = Convert.ToInt32(jsonResult["offlineGold"]);
                        int offlineLoot = Convert.ToInt32(jsonResult["offlineLoot"]);
                        int elapsedMinutes = Convert.ToInt32(jsonResult["elapsedMinutes"]);

                        _idleCanvas.Show(elapsedMinutes, offlineGold, offlineLoot, async (isAd) =>
                        {
                            if (isAd)
                            {
                                // 보상 2배 획득
                                await RequestAgainIdleReward();
                            }
                            else
                            {
                                // 보상 획득
                                SaveManager.Instance.profileData.ChangeProfileData(jsonResult["profileData"].ToString());
                            }
                        });

                        tcs.TrySetResult();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                        tcs.TrySetResult();
                    }
                },
                (PlayFabError error) =>
                {
                    DebugPlayFabError(error);
                    tcs.TrySetResult();
                });

            await tcs.Task;
        }

        /// <summary>
        /// 온라인 보상 획득 시도
        /// </summary>
        private async UniTask RequestOnlineIdleReward()
        {
            var records = _enemyRecordSystem.GetRecords();
            var tcs = new UniTaskCompletionSource();

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "RequestOnlineIdleReward",
                FunctionParameter = new { CurrentWave = _waveSystem.currentWave.id, EnemyRecords = records },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        _enemyRecordSystem.ClearRecords();
                        SaveManager.Instance.profileData.ChangeProfileData(jsonResult["profileData"].ToString());
                        tcs.TrySetResult();
                    }
                },
                (PlayFabError error) =>
                {
                    DebugPlayFabError(error);
                    tcs.TrySetResult();
                });

            await tcs.Task;
        }

        /// <summary>
        /// 보상 2배 획득 시도
        /// </summary>
        private async UniTask RequestAgainIdleReward()
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "RequestAgainIdleReward",
                FunctionParameter = new { },
                GeneratePlayStreamEvent = true
            };

            var tcs = new UniTaskCompletionSource();

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    var json = (JsonObject)result.FunctionResult;

                    if ((bool)json["success"])
                    {
                        SaveManager.Instance.profileData.ChangeProfileData(json["profileData"].ToString());
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(json["error"].ToString());
                    }

                    tcs.TrySetResult();

                },
                (PlayFabError error) =>
                {
                    DebugPlayFabError(error);
                    tcs.TrySetResult();
                });

            await tcs.Task;
        }
        #endregion

        #region 오류 디버그
        private void DebugPlayFabError(PlayFabError error)
        {
            switch (error.Error)
            {
                case PlayFabErrorCode.ConnectionError:
                case PlayFabErrorCode.ExperimentationClientTimeout:
                    UIPopupManager.Instance.ShowConfirmPopup("네트워크 연결을 확인해주세요.", () =>
                    {
                        SceneManager.LoadScene("Login");
                    });
                    break;
                case PlayFabErrorCode.ServiceUnavailable:
                    UIPopupManager.Instance.ShowConfirmPopup("게임 서버가 불안정합니다.\n나중에 다시 접속해주세요.\n죄송합니다.", () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    });
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
#endif
                    break;
            }
        }
        #endregion
    }
}