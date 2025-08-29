using FrameWork.Editor;
using FrameWork.PlayFabExtensions;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class ShopSaveData
    {
        [Tooltip("각 상점 카테고리 정보")]
        public List<ShopCatalog> ShopCatalogs = new List<ShopCatalog>();

        #region 데이터 모델
        [Serializable]
        public class ShopItem
        {
            public string ItemId;
            public int BoughtCount;

            public ShopItem(string id)
            {
                ItemId = id;
            }

            public void IncreaseCount(int amount = 1)
            {
                BoughtCount += amount;
            }
        }

        [Serializable]
        public class ShopCatalog
        {
            public string CatalogId;
            public long RefreshTick;
            public List<ShopItem> Items = new List<ShopItem>();

            public DateTime LastRefresh
            {
                get => new DateTime(RefreshTick);
                set => RefreshTick = value.Ticks;
            }

            public ShopItem GetItem(string itemId)
            {
                return Items.Find(x => x.ItemId == itemId);
            }

            public void AddItem(string catalogId, int buyCount)
            {
                var item = GetItem(catalogId);
                if (item != null)
                {
                    item.IncreaseCount(buyCount);
                }
                else
                {
                    var newItem = new ShopItem(catalogId);
                    newItem.IncreaseCount(buyCount);
                    Items.Add(newItem);
                }
            }
        }
        #endregion
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/ShopSaveData", fileName = "ShopSaveData", order = 3)]
    public class ShopSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private ShopSaveData _data;

        public static ShopTitleData shopTitleData { get; private set; }

        #region 저장 및 로드
        public override void SetDefaultValues()
        {
            _data = new ShopSaveData();

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<ShopSaveData>(json);

            if (_data != null)
            {
                isLoaded = true;
                shopTitleData = TitleDataManager.LoadShopData();

                if (_data.ShopCatalogs == null || _data.ShopCatalogs.Count != shopTitleData.subTabCount)
                {
                    InitializeShopData();
                }
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }
        #endregion

        public ShopSaveData.ShopCatalog GetShopCatalog(string id)
        {
            var shopCatalog = _data.ShopCatalogs.Find(x => x.CatalogId == id);

            return shopCatalog;
        }

        private void InitializeShopData()
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "InitializeShopData",
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                 (ExecuteCloudScriptResult result) =>
                 {
                 }, DebugPlayFabError);
        }

        public void RefreshShop(string shopId, UnityAction onComplete, UnityAction<float> onFailed)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "RefreshShop",
                FunctionParameter = new { shopId = shopId },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {

                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        JsonObject shopDataJson = (JsonObject)jsonResult["shopData"];
                        _data = JsonUtility.FromJson<ShopSaveData>(shopDataJson.ToString());

                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                        if (float.TryParse(jsonResult["remainTime"].ToString(), out float remainTime))
                        {
                            onFailed?.Invoke(remainTime);
                        }
                    }
                }, DebugPlayFabError);
        }

        public void PurchaseItem(string itemId, int buyCount, UnityAction onComplete)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "PurchaseItem",
                FunctionParameter = new { itemId = itemId, buyCount = buyCount },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if (result.FunctionResult == null)
                    {
                        UIPopupManager.Instance.ShowConfirmPopup("서버 응답이 없습니다.");
                        return;
                    }

                    if ((bool)jsonResult["success"])
                    {
                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
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