using FrameWork.Editor;
using FrameWork.PlayFabExtensions;
using FrameWork.UIPopup;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class GachaSaveData
    {
        [Tooltip("각 뽑기 카테고리 정보")]
        public List<GachaCatalog> GachaCatalogs = new List<GachaCatalog>();

        #region 데이터 모델
        [Serializable]
        public class GachaItem
        {
            public int BoughtCount;

            internal void IncreaseCount(int amount = 1)
            {
                BoughtCount += amount;
            }
        }

        [Serializable]
        public class GachaCatalog
        {
            public string CatalogId;
            public long RefreshTick;
            public List<GachaItem> Items = new List<GachaItem>();

            public DateTime LastRefresh
            {
                get => new DateTime(RefreshTick);
                set => RefreshTick = value.Ticks;
            }

            internal void AddItem(int index)
            {
                while (Items.Count <= index)
                {
                    Items.Add(null);
                }

                if (Items[index] == null)
                {
                    Items[index] = new GachaItem();
                }

                Items[index].IncreaseCount(1);
            }
        }
        #endregion
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/GachaSaveData", fileName = "GachaSaveData", order = 5)]
    public class GachaSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private GachaSaveData _data;

        public static GachaTitleData gachaTitleData { get; private set; }

        #region 저장 및 로드
        public override void SetDefaultValues()
        {
            _data = new GachaSaveData();

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<GachaSaveData>(json);

            if (_data != null)
            {
                isLoaded = true;
                gachaTitleData = TitleDataManager.LoadGachaData();
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

        #region 로컬
        public GachaSaveData.GachaCatalog GetGachaCatalog(string id)
        {
            var gachaCatalog = _data.GachaCatalogs.Find(x => x.CatalogId == id);

            return gachaCatalog;
        }

        public void AddItem(string catalogId, int itemId)
        {
            var gachaCatalog = GetGachaCatalog(catalogId);

            if (gachaCatalog == null)
            {
                gachaCatalog = new GachaSaveData.GachaCatalog
                {
                    CatalogId = catalogId,
                    RefreshTick = DateTime.UtcNow.Ticks
                };
                _data.GachaCatalogs.Add(gachaCatalog);
            }

            gachaCatalog.AddItem(itemId);
        }
        #endregion

        public void RefreshGacha(UnityAction onComplete, UnityAction<float> onFailed)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "RefreshGacha",
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        _data.GachaCatalogs.Clear();

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

        public void PickUp(string gachaTitle, int gachaCount, UnityAction<Dictionary<string, int>, string[]> onComplete)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "PlayGacha",
                FunctionParameter = new { gachaTitle = gachaTitle, gachaCount = gachaCount },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        var payCost = new Dictionary<string, int>();

                        if (jsonResult.ContainsKey("payCost"))
                        {
                            var payCostJsonStr = jsonResult["payCost"].ToString();
                            var payCostObj = JsonConvert.DeserializeObject<Dictionary<string, int>>(payCostJsonStr);

                            if (payCostObj != null)
                            {
                                payCost = payCostObj;
                            }
                        }

                        var resultsArray = (JsonArray)jsonResult["results"];
                        var rewards = resultsArray.Select(r => r.ToString()).ToArray();

                        onComplete?.Invoke(payCost, rewards);
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }

        public void AdPickUp(string gachaTitle, int gachaCount, int itemIndex, UnityAction<string[]> onComplete)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "AdPlayGacha",
                FunctionParameter = new { gachaTitle = gachaTitle, gachaCount = gachaCount, itemIndex = itemIndex },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        var resultsArray = (JsonArray)jsonResult["results"];
                        var rewards = resultsArray.Select(r => r.ToString()).ToArray();

                        onComplete?.Invoke(rewards);
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