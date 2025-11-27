using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EvolveThisMatch.Battle
{
    public class BattleResultSystem : MonoBehaviour, IBattleSystem
    {
        private EnemySystem _enemySystem;
        private BattleWaveSystem _waveSystem;
        private bool _isEnd = false;

        public BattleResultData battleResultData { get; private set; }

        public event UnityAction<bool> onBattleEnd;

        public void Initialize()
        {
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();
            _waveSystem = BattleManager.Instance.GetSubSystem<BattleWaveSystem>();

            _enemySystem.onDeregist += OnEnemyKilled;
            _enemySystem.onRegist += OnEnemySpawned;
        }

        public void Deinitialize()
        {
            _enemySystem.onDeregist -= OnEnemyKilled;
            _enemySystem.onRegist -= OnEnemySpawned;
        }

        /// <summary>
        /// 모든 적 처치 시, 승리
        /// </summary>
        private void OnEnemyKilled(Unit unit)
        {
            if (_isEnd) return;

            if (_waveSystem.isWaveEnd && _waveSystem.isSpawnEnd && _enemySystem.enemyCount == 0)
            {
                EndBattle(true);
            }
        }

        /// <summary>
        /// 적이 100마리 이상 소환되면, 승리
        /// </summary>
        private void OnEnemySpawned(Unit unit)
        {
            if (_isEnd) return;

            if (_enemySystem.enemyCount > 100)
            {
                EndBattle(false);
            }
        }

        private void EndBattle(bool battleResult)
        {
            _isEnd = true;
            _waveSystem.ForceEndWave();

            RequestBattleReward(battleResult);
        }

        private void RequestBattleReward(bool battleResult)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "BattleEnd",
                FunctionParameter = new
                {
                    BattleResult = battleResult,
                    WaveIndex = _waveSystem.currentWaveIndex
                },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject json = (JsonObject)result.FunctionResult;

                    if ((bool)json["success"])
                    {
                        int categoryIndex = Convert.ToInt32(json["category"]);
                        int chapterIndex = Convert.ToInt32(json["chapter"]);

                        var category = _waveSystem.waveLibrary.categorys[categoryIndex];
                        var chapter = category.chapters[chapterIndex];
                        var page = _waveSystem.currentWaveIndex;

                        var rewards = new List<(string, int)>();

                        var rewardData = (JsonArray)json["rewardData"];
                        foreach (JsonObject reward in rewardData)
                        {
                            string variable = reward["variable"].ToString();
                            int amount = Convert.ToInt32(reward["amount"]);
                            rewards.Add((variable, amount));
                        }

                        battleResultData = new BattleResultData(category.title, chapter.chapterName, $"p. {page}", rewards, RequestAgainBattleReward);
                        SaveManager.Instance.profileData.ChangeProfileData(json["profileData"].ToString());
                        onBattleEnd?.Invoke(battleResult);
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(json["error"].ToString());
                    }
                }, DebugPlayFabError);
        }

        private void RequestAgainBattleReward(bool isAgain)
        {
            if (!isAgain) return;

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "RequestAgainBattleReward",
                FunctionParameter = new { },
                GeneratePlayStreamEvent = true
            };

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
                }, DebugPlayFabError);
        }

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

    public class BattleResultData
    {
        public string category { get; private set; }
        public string chapter { get; private set; }
        public string finalPage { get; private set; }
        public List<(string, int)> rewardData { get; private set; }
        public UnityAction<bool> onAgainReward { get; private set; }

        public BattleResultData(string category, string chapter, string finalPage, List<(string, int)> rewardData, UnityAction<bool> onAgainReward)
        {
            this.category = category;
            this.chapter = chapter;
            this.finalPage = finalPage;
            this.rewardData = rewardData;
            this.onAgainReward = onAgainReward;
        }
    }
}