using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FrameWork.PlayFabExtensions
{
    public static class TitleDataManager
    {
        private static Dictionary<string, string> _titleData = new Dictionary<string, string>();

        public static void LoadTitleData(UnityAction onComplete = null)
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
            {
                _titleData = result.Data;
                onComplete?.Invoke();
            }, DebugPlayFabError);
        }

        public static void LoadAgentData(ref ObscuredInt[] agentTierUpRequirements, ref ObscuredInt[] agentMaxLevelPerTier, ref ObscuredInt[] foodExp)
        {
            if (!_titleData.TryGetValue("AgentData", out string json))
            {
#if UNITY_EDITOR
                Debug.LogError("AgentData를 찾을 수 없습니다.");
#endif
                return;
            }

            var agentData = JsonUtility.FromJson<AgentTitleData>(json);
            
            // ObscuredInt 배열로 변환
            agentTierUpRequirements = agentData.agentTierUpRequirements.Select(v => (ObscuredInt)v).ToArray();

            agentMaxLevelPerTier = agentData.agentMaxLevelPerTier.Select(v => (ObscuredInt)v).ToArray();

            foodExp = agentData.foodExp.Select(v => (ObscuredInt)v).ToArray();
        }

        private static void DebugPlayFabError(PlayFabError error)
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

    [Serializable]
    public class AgentTitleData
    {
        public int[] agentTierUpRequirements;
        public int[] agentMaxLevelPerTier;
        public int[] foodExp;
    }
}