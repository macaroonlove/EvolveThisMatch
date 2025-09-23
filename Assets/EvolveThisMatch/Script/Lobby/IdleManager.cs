using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
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
        private bool _isRunning = true;

        private void Start()
        {
            if (!PlayFabAuthService.IsLoginState)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _enemyRecordSystem = BattleManager.Instance.GetSubSystem<EnemyRecordSystem>();

            SendDataTimer().Forget();
        }

        private async UniTaskVoid SendDataTimer()
        {
            while (_isRunning)
            {
                await UniTask.Delay(TimeSpan.FromMinutes(5), ignoreTimeScale: true);

                await SendDataToServer();
            }
        }

        public void OnStageChange()
        {
            SendDataToServer().Forget();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) SendDataToServer().Forget();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) SendDataToServer().Forget();
        }

        private void OnApplicationQuit()
        {
            SendDataToServer().Forget();

            _isRunning = false;
        }

        private async UniTask SendDataToServer()
        {
            var records = _enemyRecordSystem.GetServerRecords();

            if (records.Length == 0) return;

            var tcs = new UniTaskCompletionSource();

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpdateIdleState",
                FunctionParameter = new { EnemyRecords = records },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        _enemyRecordSystem.ClearServerRecords();
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
    }
}