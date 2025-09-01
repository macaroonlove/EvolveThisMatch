using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UI;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaCanvas : UIBase
    {
        #region 바인딩
        enum Images
        {
            Background,
        }
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        [Header("탭")]
        [SerializeField] private GameObject _tabPrefab;
        [SerializeField] private Transform _tabParent;

        [Header("버튼")]
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private RectTransform _buttonParent;

        private List<UIGachaButton> _gachaButtons = new List<UIGachaButton>();
        private Image _background;
        private UIGachaResultCanvas _gachaResultCanvas;

        private UIGachaTab _firstTab;
        private UIGachaTab _currentTab;

        protected override void Initialize()
        {
            _gachaResultCanvas = GetComponentInChildren<UIGachaResultCanvas>();

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));

            _background = GetImage((int)Images.Background);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            SetTab();
        }

        public override void Show(bool isForce = false)
        {
            base.Show(isForce);

            Select(_firstTab);
        }

        private void SetTab()
        {
            var datas = TitleDataManager.LoadGachaData();

            bool isSelected = false;
            foreach (var data in datas.gachaCatalog)
            {
                var instance = Instantiate(_tabPrefab, _tabParent);
                var gachaTab = instance.GetComponent<UIGachaTab>();

                gachaTab.Initialize(data.Key, data.Value, Select);

                if (isSelected == false)
                {
                    _firstTab = gachaTab;
                    isSelected = true;
                }
            }
        }

        private void Select(UIGachaTab tab)
        {
            _currentTab?.UnSelect();
            _currentTab = tab;
            _currentTab?.Select();

            // 배경 적용
            AddressableAssetManager.Instance.GetSprite(tab.gachaData.background, (background) =>
            {
                _background.sprite = background;
            });

            // 버튼 최신화
            SetButtons(tab);

            // 좌측 상단에 보여질 재화 최신화
            RefreshVariableDisplay(tab);
        }

        private void SetButtons(UIGachaTab tab)
        {
            var costs = tab.gachaData.costs;
            var infos = tab.gachaData.buttons;
            int count = infos.Count;

            // 필요한 수만큼 생성
            while (_gachaButtons.Count < count)
            {
                var instance = Instantiate(_buttonPrefab, _buttonParent);
                var gachaButton = instance.GetComponent<UIGachaButton>();
                _gachaButtons.Add(gachaButton);
            }

            // 초기화
            for (int i = 0; i < _gachaButtons.Count; i++)
            {
                if (i < count)
                {
                    var info = infos[i];
                    _gachaButtons[i].Show(info.count, costs, info.color, PickUp);
                }
                else
                {
                    _gachaButtons[i].Hide();
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonParent);
        }

        private async void RefreshVariableDisplay(UIGachaTab tab)
        {
            VariableDisplayManager.Instance.HideAll();

            if (tab.gachaData.additionalVariable != null)
            {
                var currency = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(tab.gachaData.additionalVariable);
                if (currency != null)
                {
                    VariableDisplayManager.Instance.Show(currency);
                }
            }
            foreach (var cost in tab.gachaData.costs)
            {
                var currency = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(cost.costVariable);
                if (currency != null)
                {
                    VariableDisplayManager.Instance.Show(currency);
                }
            }
        }

        private void PickUp(int gachaCount)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "PlayGacha",
                FunctionParameter = new { gachaTitle = _currentTab.gachaTitle, gachaCount = gachaCount },
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

                        PickUpAfter(payCost, rewards);
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }

        private async void PickUpAfter(Dictionary<string, int> payCost, string[] rewards)
        {
            foreach (var cost in payCost)
            {
                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(cost.Key);
                variable.AddValue(-cost.Value);
            }

            foreach (var reward in rewards)
            {
                var parts = reward.Split('_');
                string type = parts[0];
                int id = int.Parse(parts[1]);

                if (type == "Agent")
                {
                    SaveManager.Instance.agentData.AddAgent(id);
                }
                else if (type == "Artifact")
                {
                    SaveManager.Instance.itemData.AddArtifact(id);
                }
                else if (type == "Tome")
                {
                    SaveManager.Instance.itemData.AddTome(id);
                }
            }

            SetButtons(_currentTab);

            _gachaResultCanvas.Show(rewards);
        }

        public void Hide()
        {
            VariableDisplayManager.Instance.HideAll();

            base.Hide(true);
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