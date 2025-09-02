using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.NetworkTime;
using FrameWork.PlayFabExtensions;
using FrameWork.Service;
using FrameWork.UI;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using ScriptableObjectArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIShopCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
        }
        enum Texts
        {
            RemainTimeText,
        }
        enum Images
        {
            Background,
        }
        #endregion

        [Header("메인 탭")]
        [SerializeField] private GameObject _mainTabPrefab;
        [SerializeField] private Transform _mainTabParent;

        [Header("서브 탭")]
        [SerializeField] private GameObject _subTabPrefab;
        [SerializeField] private Transform _subTabParent;

        [Header("아이템")]
        [SerializeField] private GameObject _packageItemPrefab;
        [SerializeField] private GameObject _defaultItemPrefab;
        [SerializeField] private Transform _itemParent;

        [Header("획득 아이템")]
        [SerializeField] private GameObject _rewardItemPrefab;

        private TextMeshProUGUI _remainTime;
        private Image _background;
        private UIShopMainTab _currentTab;
        private UIPackagePayPanel _packagePayPanel;
        private UIDefaultPayPanel _defaultPayPanel;

        private PoolSystem _poolSystem;
        private CurrencySystem _currencySystem;

        private List<UIShopMainTab> _shopMainTabs = new List<UIShopMainTab>();
        private List<UIShopSubTab> _shopSubTabs = new List<UIShopSubTab>();
        private List<UIDefaultShopItem> _defaultShopItems = new List<UIDefaultShopItem>();
        private List<UIPackageShopItem> _packageShopItems = new List<UIPackageShopItem>();
        private Dictionary<GameObject, UIShopRewardItem> _shopRewardItems = new Dictionary<GameObject, UIShopRewardItem>();

        private Coroutine _refreshCoroutine;
        private WaitForSecondsRealtime _refreshMinuteWFS = new WaitForSecondsRealtime(60);
        private WaitForSecondsRealtime _refreshSecondWFS = new WaitForSecondsRealtime(1);

        protected async override void Initialize()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _currencySystem = CoreManager.Instance.GetSubSystem<CurrencySystem>();
            _packagePayPanel = GetComponentInChildren<UIPackagePayPanel>();
            _defaultPayPanel = GetComponentInChildren<UIDefaultPayPanel>();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _remainTime = GetText((int)Texts.RemainTimeText);
            _background = GetImage((int)Images.Background);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            await UniTask.WaitUntil(() => SaveManager.Instance.shopData.isLoaded);
            CreateTabs();
            CreateItems();
        }

        public override void Show(bool isForce = false)
        {
            base.Show(isForce);

            SelectMainTab(_shopMainTabs[0]);
        }

        #region 생성
        private void CreateTabs()
        {
            var mainTabDatas = ShopSaveDataTemplate.shopTitleData.shopCatalog;

            foreach (var mainTabData in mainTabDatas)
            {
                var mainTab = InstantiatePrefab(_mainTabPrefab, _mainTabParent, _shopMainTabs);
                mainTab.Initialize(mainTabData.Key, mainTabData.Value, SelectMainTab);
            }

            for (int i = 0; i < 5; i++)
            {
                InstantiatePrefab(_subTabPrefab, _subTabParent, _shopSubTabs);
            }
        }

        private void CreateItems()
        {
            for (int i = 0; i < 10; i++)
            {
                InstantiatePrefab(_packageItemPrefab, _itemParent, _packageShopItems);
            }

            for (int i = 0; i < 5; i++)
            {
                var instance = Instantiate(_defaultItemPrefab, _itemParent);
                var defaultItems = instance.GetComponentsInChildren<UIDefaultShopItem>();

                foreach (var defaultItem in defaultItems)
                {
                    _defaultShopItems.Add(defaultItem);
                }
            }
        }

        private T InstantiatePrefab<T>(GameObject prefab, Transform parent, List<T> list) where T : Component
        {
            var instance = Instantiate(prefab, parent);
            var component = instance.GetComponent<T>();
            list.Add(component);
            return component;
        }
        #endregion

        #region 탭 선택

        #region 메인
        private void SelectMainTab(UIShopMainTab tab)
        {
            _currentTab?.UnSelect();
            _currentTab = tab;
            _currentTab?.Select();

            // 배경 적용
            AddressableAssetManager.Instance.GetSprite(tab.mainTabData.mainTabBackground, (background) =>
            {
                _background.sprite = background;
            });

            // 서브 탭 최신화
            RefreshSubTabs(tab);
        }

        private void RefreshSubTabs(UIShopMainTab tab)
        {
            var datas = tab.mainTabData.subTabGroup;

            for (int i = 0; i < _shopSubTabs.Count; i++)
            {
                if (i < datas.Count)
                {
                    _shopSubTabs[i].Show(datas[i], SelectSubTab);
                    _shopSubTabs[i].UnSelect();
                }
                else
                {
                    _shopSubTabs[i].Hide(true);
                }
            }

            _shopSubTabs[0].Select();

            _subTabParent.parent.gameObject.SetActive(datas.Count > 1);
        }
        #endregion

        #region 서브
        private void SelectSubTab(UIShopSubTab tab)
        {
            // 모든 탭 비활성화
            foreach (var subTab in _shopSubTabs) subTab.UnSelect();

            // 모든 아이템 비활성화
            HideAllItems();

            var shopCatalog = SaveManager.Instance.shopData.GetShopCatalog(tab.subTabData.subTab);

            // 상점 최신화 타이머
            RefreshShopTimer(tab, shopCatalog);

            // 아이템 최신화
            RefreshItems(tab, shopCatalog);

            // 좌측 상단에 보여질 재화 최신화
            RefreshVariableDisplay(tab.subTabData);
        }

        #region 아이템 최신화
        private void RefreshItems(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog)
        {
            var items = tab.subTabData.items;

            int pIndex = 0;
            int dIndex = 0;
            foreach (var item in items)
            {
                if (item.isPackage)
                {
                    var shopRewardItemList = GetShopRewardItem(item.rewards.Count);

                    _packageShopItems[pIndex].Show(shopCatalog, item, shopRewardItemList, () => SelectPackage(tab, shopCatalog, item));
                    pIndex++;
                }
                else
                {
                    _defaultShopItems[dIndex].Show(shopCatalog, item, () => SelectDefault(tab, shopCatalog, item));
                    dIndex++;
                }
            }
        }

        private void HideAllItems()
        {
            foreach (var item in _packageShopItems) item.Hide(true);
            foreach (var item in _defaultShopItems) item.Hide(true);
            foreach (var item in _shopRewardItems.Keys) _poolSystem.DeSpawn(item);
        }

        private List<UIShopRewardItem> GetShopRewardItem(int count)
        {
            var results = new List<UIShopRewardItem>();

            for (int i = 0; i < count; i++)
            {
                var instance = _poolSystem.Spawn(_rewardItemPrefab);

                if (!_shopRewardItems.TryGetValue(instance, out var rewardItem))
                {
                    rewardItem = instance.GetComponent<UIShopRewardItem>();
                    _shopRewardItems.Add(instance, rewardItem);
                }

                results.Add(rewardItem);
            }

            return results;
        }
        #endregion

        private async void RefreshVariableDisplay(ShopSubTab subTabData)
        {
            VariableDisplayManager.Instance.HideAll();

            foreach (var showVariable in subTabData.showVariable)
            {
                var currency = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(showVariable);
                VariableDisplayManager.Instance.Show(currency);
            }
        }
        #endregion

        #endregion

        #region 상점 최신화
        private async void RefreshShopTimer(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog)
        {
            _remainTime.transform.parent.gameObject.SetActive(false);

            // 코루틴이 존재한다면 멈추기
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
                _refreshCoroutine = null;
            }

            var data = tab.subTabData;

            // 상점 최신화 주기가 존재한다면
            if (data.resetInterval > 0)
            {
                var remainTime = await NetworkTimeManager.Instance.GetRemainTime(shopCatalog.LastRefresh, Enum.Parse<ECycleType>(data.resetType), data.resetInterval);

                // 아직 최신화 할 때가 되지 않았다면
                if (remainTime.TotalSeconds > 0)
                {
                    _refreshCoroutine = StartCoroutine(CoRefreshShopTimer(remainTime, tab));
                }
                // 최신화 할 때라면
                else
                {
                    RefreshShop(tab);
                }
            }
        }

        #region 상점 최신화까지 남은 시간 대기 및 완료되었을 때 요청 보내기
        private IEnumerator CoRefreshShopTimer(TimeSpan remainTime, UIShopSubTab tab)
        {
            if (remainTime.TotalSeconds <= 0)
            {
                RefreshShop(tab);
                yield break;
            }

            // 상점에 표시할 것이라면
            if (tab.subTabData.isShowTime)
            {
                _remainTime.transform.parent.gameObject.SetActive(true);

                // 1분 중, 잔여 초 부터 정리
                if (remainTime.TotalSeconds > 60)
                {
                    double remainder = remainTime.TotalSeconds % 60;
                    double firstWait = remainder > 0 ? remainder : 60;

                    // 남은 시간 UI표시
                    UpdateRemainTime(remainTime);

                    yield return new WaitForSecondsRealtime((float)firstWait);

                    remainTime -= TimeSpan.FromSeconds(firstWait);
                }

                // 1분마다 표시하기
                while (remainTime.TotalSeconds > 0)
                {
                    // 남은 시간 UI표시
                    UpdateRemainTime(remainTime);

                    if (remainTime.TotalSeconds > 60)
                    {
                        yield return _refreshMinuteWFS;

                        // 남은 시간 업데이트
                        float waitSeconds = Mathf.Min(60f, (float)remainTime.TotalSeconds);
                        remainTime -= TimeSpan.FromSeconds(waitSeconds);
                    }
                    else
                    {
                        yield return _refreshSecondWFS;

                        // 남은 시간 업데이트
                        remainTime -= TimeSpan.FromSeconds(1);
                    }
                }
            }
            // 상점에 표시하지 않을거라면 한 번에 대기하기
            else
            {
                yield return new WaitForSecondsRealtime((float)remainTime.TotalSeconds);
            }

            RefreshShop(tab);
        }

        private void UpdateRemainTime(TimeSpan remainTime)
        {
            if (remainTime.TotalDays >= 1) _remainTime.text = $"{(int)remainTime.TotalDays}일 {remainTime.Hours:D2}시간";
            else if (remainTime.TotalSeconds > 60) _remainTime.text = $"{remainTime.Hours:D2}시간 {remainTime.Minutes:D2}분";
            else _remainTime.text = $"{remainTime.Minutes:D2}분 {remainTime.Seconds:D2}초";
        }
        #endregion

        private void RefreshShop(UIShopSubTab tab)
        {
            SaveManager.Instance.shopData.RefreshShop(tab.subTabData.subTab, () =>
            {
                // 탭 내용물 다시 보여주기
                tab.Select();
            }, (remainTime) =>
            {
                _refreshCoroutine = StartCoroutine(CoRefreshShopTimer(TimeSpan.FromSeconds(remainTime), tab));
            });
        }
        #endregion

        #region 아이템 선택
        private void SelectPackage(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog, ShopItem itemData)
        {
            if (!IsPayAble(itemData)) return;

            if (itemData.isOpenPanel)
            {
                _packagePayPanel.Show(shopCatalog, itemData, () => Pay(tab, itemData));
            }
            else
            {
                Pay(tab, itemData);
            }
        }

        private void SelectDefault(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog, ShopItem itemData)
        {
            if (!IsPayAble(itemData)) return;

            if (itemData.isOpenPanel)
            {
                _defaultPayPanel.Show(shopCatalog, itemData, (buyCount) => Pay(tab, itemData, buyCount));
            }
            else
            {
                Pay(tab, itemData);
            }
        }

        /// <summary>
        /// 구매 가능 여부
        /// </summary>
        private bool IsPayAble(ShopItem itemData)
        {
            if (itemData.currency != "RM" && itemData.price != 0)
            {
                if (Enum.TryParse<CurrencyType>(itemData.currency, true, out var currency))
                {
                    var value = _currencySystem.GetAmount(currency);
                    int maxValue = value / itemData.price;

                    if (maxValue <= 0)
                    {
                        UIPopupManager.Instance.ShowConfirmPopup("재화가 부족해서 구매가 불가능합니다.");
                        return false;
                    }
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("재화가 등록되어있지 않습니다.");
#endif
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region 결제
        private async void Pay(UIShopSubTab tab, ShopItem itemData, int buyCount = 1)
        {
            if (itemData.currency == "RM" && itemData.price > 0)
            {
                var productId = itemData.id.ToLower();
                var receipt = await IAPManager.Instance.PurchaseProductAsync(productId);

                if (receipt != null)
                {
                    SaveManager.Instance.shopData.PurchaseItemRM(itemData.id, receipt, (rewards) =>
                    {
                        PayAfter(tab, itemData, buyCount, rewards);
                    });
                }
                else
                {
                    UIPopupManager.Instance.ShowConfirmPopup("결제에 실패하였습니다.");
                }
            }
            else
            {
                SaveManager.Instance.shopData.PurchaseItem(itemData.id, buyCount, (rewards) =>
                {
                    PayAfter(tab, itemData, buyCount, rewards);
                });
            }
        }

        private async void PayAfter(UIShopSubTab tab, ShopItem itemData, int buyCount, List<ShopSaveDataTemplate.ShopReward> rewards)
        {
            // 비용 처리
            var currency = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(itemData.currency);
            currency.AddValue(-itemData.price * buyCount);

            // 보상 처리
            foreach (var reward in rewards)
            {
                if (reward.type == "Profile")
                {
                    var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(reward.key);
                    variable.AddValue(reward.amount);
                }
                else if (reward.type == "Agent")
                {
                    SaveManager.Instance.agentData.AddAgent(reward.id, reward.amount);
                }
                else if (reward.type == "Artifact")
                {
                    SaveManager.Instance.itemData.AddArtifact(reward.id, reward.amount);
                }
                else if (reward.type == "Tome")
                {
                    SaveManager.Instance.itemData.AddTome(reward.id, reward.amount);
                }
                else if (reward.type == "Method")
                {
                    // 뽑기 결과를 로컬에 반영
                    foreach (var result in reward.results)
                    {
                        var parts = result.Split('_');
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
                }
            }

            // 구매 처리
            var shopCatalog = SaveManager.Instance.shopData.GetShopCatalog(tab.subTabData.subTab);
            shopCatalog.AddItem(itemData.id, buyCount);

            tab.Select();
        }
        #endregion

        public void Hide()
        {
            VariableDisplayManager.Instance.HideAll();

            base.Hide(true);
        }
    }
}