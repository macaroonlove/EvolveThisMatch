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
        public List<ShopCatalog> shopCatalogs = new List<ShopCatalog>();

        #region 데이터 모델
        [Serializable]
        public class ShopItem
        {
            [SerializeField] private string _itemId;
            [SerializeField] private int _boughtCount;

            public string itemId => _itemId;
            public int boughtCount => _boughtCount;

            public ShopItem(string id)
            {
                _itemId = id;
            }

            public void IncreaseCount(int amount = 1)
            {
                _boughtCount += amount;
            }
        }

        [Serializable]
        public class ShopCatalog
        {
            [SerializeField] private string _catalogId;
            [SerializeField] private long _lastBuyTicks;
            [SerializeField] private List<ShopItem> _items = new List<ShopItem>();

            public string catalogId => _catalogId;
            public IReadOnlyList<ShopItem> items => _items;
            public DateTime lastBuyTime
            {
                get => new DateTime(_lastBuyTicks);
                set => _lastBuyTicks = value.Ticks;
            }

            public ShopCatalog(string id)
            {
                _catalogId = id;
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
                    _items.Add(newItem);
                }
            }

            public ShopItem GetItem(string itemId)
            {
                return _items.Find(x => x.itemId == itemId);
            }

            public void ResetAllItems()
            {
                _items.Clear();
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
            var catalog = _data.shopCatalogs.Find(x => x.catalogId == id);

            if (catalog == null)
            {
                catalog = new ShopSaveData.ShopCatalog(id);
                _data.shopCatalogs.Add(catalog);
            }

            return catalog;
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
                (ExecuteCloudScriptResult result) => {

                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

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