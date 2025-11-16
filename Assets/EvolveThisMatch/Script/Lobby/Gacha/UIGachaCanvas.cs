using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.NetworkTime;
using FrameWork.Service;
using FrameWork.UI;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private GameObject _adButtonPrefab;
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private RectTransform _buttonParent;

        private List<UIGachaAdButton> _gachaAdButtons = new List<UIGachaAdButton>();
        private List<UIGachaButton> _gachaButtons = new List<UIGachaButton>();
        private Image _background;
        private UIGachaResultCanvas _gachaResultCanvas;

        private UIGachaTab _firstTab;
        private UIGachaTab _currentTab;

        private Coroutine _refreshCoroutine;
        private UnityAction _onClose;

        protected async override void Initialize()
        {
            _gachaResultCanvas = GetComponentInChildren<UIGachaResultCanvas>();

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));

            _background = GetImage((int)Images.Background);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            //if (PlayFabAuthService.IsLoginState)
            await UniTask.WaitUntil(() => SaveManager.Instance.shopData.isLoaded);

            SetTab();
        }

        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            base.Show(true);

            Select(_firstTab);
        }

        private void Hide()
        {
            _onClose?.Invoke();

            base.Hide(true);
        }

        private void SetTab()
        {
            var datas = GachaSaveDataTemplate.gachaTitleData;

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

            // 좌측 상단에 보여질 재화 최신화
            RefreshVariableDisplay(tab);

            var gachaCatalog = SaveManager.Instance.gachaData.GetGachaCatalog(tab.gachaTitle);

            // 버튼 최신화
            SetButtons(tab, gachaCatalog);

            // 뽑기창 최신화
            RefreshGachaTimer(tab, gachaCatalog);
        }

        #region 버튼 최신화
        private void SetButtons(UIGachaTab tab, GachaSaveData.GachaCatalog gachaCatalog)
        {
            // 광고 버튼
            SetAdButtons(tab, gachaCatalog);

            // 기본 버튼
            SetDefaultButtons(tab);

            // 레이아웃 최신화
            LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonParent);
        }

        private void SetAdButtons(UIGachaTab tab, GachaSaveData.GachaCatalog gachaCatalog)
        {
            var infos = tab.gachaData.adButtons;
            int count = infos.Count;

            // 필요한 수만큼 생성
            while (_gachaAdButtons.Count < count)
            {
                var instance = Instantiate(_adButtonPrefab, _buttonParent);
                var gachaAdButton = instance.GetComponent<UIGachaAdButton>();
                _gachaAdButtons.Add(gachaAdButton);
            }

            // 초기화
            for (int i = 0; i < _gachaAdButtons.Count; i++)
            {
                if (i < count)
                {
                    var info = infos[i];
                    int boughtCount = (gachaCatalog == null || i >= gachaCatalog.Items.Count || gachaCatalog.Items[i] == null) ? 0 : gachaCatalog.Items[i].BoughtCount;
                    _gachaAdButtons[i].Show(info.count, boughtCount, info.buyAbleCount, info.color, i, (gachaCount, itemIndex) => AdPickUp(gachaCount, itemIndex, tab.gachaTitle));
                }
                else
                {
                    _gachaAdButtons[i].Hide();
                }
            }
        }

        private void SetDefaultButtons(UIGachaTab tab)
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
        }
        #endregion

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

        #region 뽑기창 최신화
        private async void RefreshGachaTimer(UIGachaTab tab, GachaSaveData.GachaCatalog gachaCatalog)
        {
            // 코루틴이 존재한다면 멈추기
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
                _refreshCoroutine = null;
            }

            // 광고 버튼이 존재한다면
            if (gachaCatalog != null && tab.gachaData.adButtons.Count > 0)
            {
                var remainTime = await NetworkTimeManager.Instance.GetRemainTime(gachaCatalog.LastRefresh, ECycleType.Daily, 1);

                // 아직 최신화 할 때가 되지 않았다면
                if (remainTime.TotalSeconds > 0)
                {
                    _refreshCoroutine = StartCoroutine(CoRefreshGachaTimer(remainTime, tab));
                }
                // 최신화 할 때라면
                else
                {
                    RefreshGacha(tab);
                }
            }
        }

        #region 뽑기 최신화까지 남은 시간 대기 및 완료되었을 때 요청 보내기
        private IEnumerator CoRefreshGachaTimer(TimeSpan remainTime, UIGachaTab tab)
        {
            if (remainTime.TotalSeconds <= 0)
            {
                RefreshGacha(tab);
                yield break;
            }

            yield return new WaitForSecondsRealtime((float)remainTime.TotalSeconds);

            RefreshGacha(tab);
        }
        #endregion

        private void RefreshGacha(UIGachaTab tab)
        {
            SaveManager.Instance.gachaData.RefreshGacha(() =>
            {
                // 탭 내용물 다시 보여주기
                tab.Select();
            }, (remainTime) =>
            {
                _refreshCoroutine = StartCoroutine(CoRefreshGachaTimer(TimeSpan.FromSeconds(remainTime), tab));
            });
        }
        #endregion

        #region 뽑기
        private void PickUp(int gachaCount)
        {
            SaveManager.Instance.gachaData.PickUp(_currentTab.gachaTitle, gachaCount, async (payCost, rewards) =>
            {
                // 비용 지불
                foreach (var cost in payCost)
                {
                    var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(cost.Key);
                    variable.AddValue(-cost.Value);
                }

                PickUpAfter(rewards);
            });
        }

        private void AdPickUp(int gachaCount, int itemIndex, string catalogId)
        {
            #if !UNITY_EDITOR
            AdmobManager.Instance.ShowRewardAd((isSuccess) =>
            {
                SaveManager.Instance.gachaData.AdPickUp(_currentTab.gachaTitle, gachaCount, itemIndex, (rewards) =>
                {
                    // 구매 횟수 증가
                    SaveManager.Instance.gachaData.AddItem(catalogId, itemIndex);

                    PickUpAfter(rewards);
                });
            });
            #endif
        }

        private void PickUpAfter(string[] rewards)
        {
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
                else
                {
                    AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(type, (variable) =>
                    {
                        variable.AddValue(id);
                    });
                }
            }

            Select(_currentTab);

            _gachaResultCanvas.Show(rewards);
        }
        #endregion
    }
}