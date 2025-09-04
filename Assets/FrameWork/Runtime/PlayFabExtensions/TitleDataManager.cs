using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.UIPopup;
using Newtonsoft.Json;
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

        public static void LoadItemData(ref ObscuredInt[] artifactLevelUpRequirements, ref ObscuredInt[] tomeLevelUpRequirements)
        {
            if (!_titleData.TryGetValue("ItemData", out string json))
            {
#if UNITY_EDITOR
                Debug.LogError("ItemData를 찾을 수 없습니다.");
#endif
                return;
            }

            var itemData = JsonUtility.FromJson<ItemTitleData>(json);

            // ObscuredInt 배열로 변환
            artifactLevelUpRequirements = itemData.artifactLevelUpRequirements.Select(v => (ObscuredInt)v).ToArray();

            tomeLevelUpRequirements = itemData.tomeLevelUpRequirements.Select(v => (ObscuredInt)v).ToArray();
        }

        public static ShopTitleData LoadShopData()
        {
            if (!_titleData.TryGetValue("ShopData", out string json))
            {
#if UNITY_EDITOR
                Debug.LogError("ShopData를 찾을 수 없습니다.");
#endif
                return null;
            }

            var shopData = JsonConvert.DeserializeObject<ShopTitleData>(json);

            return shopData;
        }

        public static GachaTitleData LoadGachaData()
        {
            if (!_titleData.TryGetValue("GachaData", out string json))
            {
#if UNITY_EDITOR
                Debug.LogError("GachaData를 찾을 수 없습니다.");
#endif
                return null;
            }

            var gachaData = JsonConvert.DeserializeObject<GachaTitleData>(json);
            
            return gachaData;
        }

        #region 오류 처리
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
        #endregion
    }

    #region AgentTitleData
    [Serializable]
    public class AgentTitleData
    {
        public int[] agentTierUpRequirements;
        public int[] agentMaxLevelPerTier;
        public int[] foodExp;
    }
    #endregion

    #region ItemTitleData
    [Serializable]
    public class ItemTitleData
    {
        public int[] artifactLevelUpRequirements;
        public int[] tomeLevelUpRequirements;
    }
    #endregion

    #region ShopTitleData
    [Serializable]
    public class ShopTitleData
    {
        public Dictionary<string, ShopMainTab> shopCatalog;

        public int subTabCount
        {
            get
            {
                int count = 0;

                foreach (var mainTab in shopCatalog)
                {
                    count += mainTab.Value.subTabGroup.Count;
                }

                return count;
            }
        }
    }

    [Serializable]
    public class ShopMainTab
    {
        public string mainTabBackground;
        public List<ShopSubTab> subTabGroup;
    }

    [Serializable]
    public class ShopSubTab
    {
        public string subTab;
        public string shopType;
        public List<string> showVariable;
        public string resetType;
        public int resetInterval;
        public bool isShowTime;
        public List<ShopItem> items;
    }

    [Serializable]
    public class ShopItem
    {
        public string id;
        public string displayName;
        public string icon;
        public string currency;
        public int price;
        public int buyAbleCount;
        public bool isPackage;
        public bool isOpenPanel;
        public List<Reward> rewards;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string key;
        public int amount;
    }
    #endregion
    
    [Serializable]
    public class GachaTitleData
    {
        public Dictionary<string, GachaData> gachaCatalog;
    }

    [Serializable]
    public class GachaData
    {
        public string background;
        public string confirmedPickUp;
        public string additionalVariable;
        public List<GachaAdButton> adButtons;
        public List<GachaButton> buttons;
        public List<GachaCost> costs;
        public List<GachaTableItem> tables;
    }

    [Serializable]
    public class GachaAdButton
    {
        public int count;
        public string color;
        public int buyAbleCount;
    }
    
    [Serializable]
    public class GachaButton
    {
        public int count;
        public string color;
    }

    [Serializable]
    public class GachaCost
    {
        public string costVariable;
        public int price;
    }

    [Serializable]
    public class GachaTableItem
    {
        public string id;
        public double rate;
    }
}