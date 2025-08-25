using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.NetworkTime;
using FrameWork.UI;
using FrameWork.UIBinding;
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
        [SerializeField] private GameObject _gainItemPrefab;

        [Header("상점 데이터")]
        [SerializeField] private ShopDataTemplateLibrary _shopDataTemplateLibrary;

        private PoolSystem _poolSystem;
        private List<UIShopSubTab> _shopSubTabList = new List<UIShopSubTab>();
        private List<UIDefaultShopItem> _defaultShopItems = new List<UIDefaultShopItem>();
        private List<UIPackageShopItem> _packageShopItems = new List<UIPackageShopItem>();
        private Dictionary<GameObject, UIShopGainItem> _shopGainItems = new Dictionary<GameObject, UIShopGainItem>();
        private UIShopMainTab _firstMainTab;
        private UIShopMainTab _currentTab;
        private UIPackagePayPanel _packagePayPanel;
        private UIDefaultPayPanel _defaultPayPanel;

        private TextMeshProUGUI _remainTime;
        private Image _background;
        private Coroutine _refreshCoroutine;
        private WaitForSecondsRealtime _refreshMinuteWFS = new WaitForSecondsRealtime(60);
        private WaitForSecondsRealtime _refreshSecondWFS = new WaitForSecondsRealtime(1);

        protected override void Initialize()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _packagePayPanel = GetComponentInChildren<UIPackagePayPanel>();
            _defaultPayPanel = GetComponentInChildren<UIDefaultPayPanel>();

            InitializeItem();
            InitializeTab();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _remainTime = GetText((int)Texts.RemainTimeText);
            _background = GetImage((int)Images.Background);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
        }

        public override void Show(bool isForce = false)
        {
            base.Show(isForce);

            SelectMainTab(_firstMainTab);
        }

        #region 생성
        private void InitializeTab()
        {
            if (_shopDataTemplateLibrary == null) return;

            var templates = _shopDataTemplateLibrary.shopDataTemplates;

            for (int i = 0; i < 5; i++)
            {
                var instance = Instantiate(_subTabPrefab, _subTabParent);
                var subTab = instance.GetComponent<UIShopSubTab>();

                _shopSubTabList.Add(subTab);
            }

            bool isSelected = false;
            foreach (var template in templates)
            {
                var instance = Instantiate(_mainTabPrefab, _mainTabParent);
                var mainTab = instance.GetComponent<UIShopMainTab>();
                mainTab.Initialize(template, SelectMainTab);

                var datas = template.shopDatas;

                if (isSelected == false)
                {
                    _firstMainTab = mainTab;
                    isSelected = true;
                }
            }
        }

        private void InitializeItem()
        {
            for (int i = 0; i < 10; i++)
            {
                var instance = Instantiate(_packageItemPrefab, _itemParent);
                var packageItem = instance.GetComponent<UIPackageShopItem>();

                _packageShopItems.Add(packageItem);
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
        #endregion

        #region 탭 선택
        private void SelectMainTab(UIShopMainTab tab)
        {
            _currentTab?.UnSelect();
            _currentTab = tab;
            _currentTab?.Select();

            _background.sprite = tab.template.background;

            var datas = tab.template.shopDatas;
            for (int i = 0; i < 5; i++)
            {
                if (i < datas.Count)
                {
                    _shopSubTabList[i].Show(datas[i], SelectSubTab);
                    _shopSubTabList[i].UnSelect();
                }
                else
                {
                    _shopSubTabList[i].Hide(true);
                }
            }

            _shopSubTabList[0].Select();

            if (datas.Count <= 1)
            {
                _subTabParent.parent.gameObject.SetActive(false);
            }
            else
            {
                _subTabParent.parent.gameObject.SetActive(true);
            }
        }

        private async void SelectSubTab(UIShopSubTab tab)
        {
            #region 탭과 아이템 모두 비활성화
            // 모든 탭 비활성화
            foreach (var subTab in _shopSubTabList) subTab.UnSelect();

            // 모든 아이템 비활성화
            foreach (var item in _packageShopItems) item.Hide(true);
            foreach (var item in _defaultShopItems) item.Hide(true);
            foreach (var item in _shopGainItems.Keys) _poolSystem.DeSpawn(item);
            #endregion

            var data = tab.data;
            var shopCatalog = SaveManager.Instance.shopData.GetShopCatalog(data.subTabName);

            #region 상점 최신화
            _remainTime.transform.parent.gameObject.SetActive(false);

            // 코루틴이 존재한다면 멈추기
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
                _refreshCoroutine = null;
            }

            // 상점 최신화 주기가 존재한다면
            if (data.cycleInterval > 0)
            {
                var remainTime = await NetworkTimeManager.Instance.GetRemainTime(shopCatalog.lastBuyTime, data.cycleType, data.cycleInterval);

                // 아직 최신화 할 때가 되지 않았다면
                if (remainTime.TotalSeconds > 0)
                {
                    _refreshCoroutine = StartCoroutine(WaitRefresh(remainTime, tab, shopCatalog));
                }
                // 최신화 할 때라면
                else
                {
                    Refresh(tab, shopCatalog).Forget();
                }
            }
            #endregion

            #region 아이템 초기화
            var items = tab.data.GetItems();

            int pIndex = 0;
            int dIndex = 0;
            foreach (var item in items)
            {
                if (item.isPackage)
                {
                    var gainItemList = GetGainItem(item.gainShopItemDatas.Count);

                    _packageShopItems[pIndex].Show(shopCatalog, item, gainItemList, (shopItem) => SelectPackage(tab, shopCatalog, shopItem, item));
                    pIndex++;
                }
                else
                {
                    _defaultShopItems[dIndex].Show(shopCatalog, item, (shopItem) => SelectDefault(tab, shopCatalog, shopItem, item));
                    dIndex++;
                }
            }
            #endregion

            #region Variable Display 설정
            VariableDisplayManager.Instance.HideAll();

            foreach (var variableDisplay in tab.data.variableDisplays)
            {
                VariableDisplayManager.Instance.Show(variableDisplay);
            }
            #endregion
        }

        private List<UIShopGainItem> GetGainItem(int count)
        {
            var results = new List<UIShopGainItem>();

            for (int i = 0; i < count; i++)
            {
                var instance = _poolSystem.Spawn(_gainItemPrefab);

                if (!_shopGainItems.TryGetValue(instance, out var gainItem))
                {
                    gainItem = instance.GetComponent<UIShopGainItem>();
                    _shopGainItems.Add(instance, gainItem);
                }

                results.Add(gainItem);
            }

            return results;
        }
        #endregion

        #region 상점 최신화
        private IEnumerator WaitRefresh(TimeSpan remainTime, UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog)
        {
            if (remainTime.TotalSeconds <= 0)
            {
                Refresh(tab, shopCatalog).Forget();
                yield break;
            }

            // 상점에 표시할 것이라면
            if (tab.data.isShowRemainTime)
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

            Refresh(tab, shopCatalog).Forget();
        }

        private void UpdateRemainTime(TimeSpan remainTime)
        {
            if (remainTime.TotalDays >= 1) _remainTime.text = $"{(int)remainTime.TotalDays}일 {remainTime.Hours:D2}시간";
            else if (remainTime.TotalSeconds > 60) _remainTime.text = $"{remainTime.Hours:D2}시간 {remainTime.Minutes:D2}분";
            else _remainTime.text = $"{remainTime.Minutes:D2}분 {remainTime.Seconds:D2}초";
        }

        private async UniTaskVoid Refresh(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog)
        {
            // 해당 탭의 모든 아이템의 구매 횟수 초기화
            shopCatalog.ResetAllItems();

            // 해당 탭의 시간을 초기화
            shopCatalog.lastBuyTime = await NetworkTimeManager.Instance.GetKoreanNow();

            // 탭 내용물 다시 보여주기
            tab.Select();

            // 저장
            _ = SaveManager.Instance.SaveData(SaveKey.Shop);
        }
        #endregion

        #region 아이템 선택
        private void SelectPackage(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog, ShopSaveData.ShopItem shopItem, ShopItemData itemData)
        {
            if (!IsPayAble(itemData)) return;

            if (itemData.isPanel)
            {
                _packagePayPanel.Show(shopCatalog, itemData, () => Pay(tab, shopCatalog, shopItem, itemData));
            }
            else
            {
                Pay(tab, shopCatalog, shopItem, itemData);
            }
        }

        private void SelectDefault(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog, ShopSaveData.ShopItem shopItem, ShopItemData itemData)
        {
            if (!IsPayAble(itemData)) return;

            if (itemData.isPanel)
            {
                _defaultPayPanel.Show(shopCatalog, itemData, (buyCount) => Pay(tab, shopCatalog, shopItem, itemData, buyCount));
            }
            else
            {
                Pay(tab, shopCatalog, shopItem, itemData);
            }
        }

        /// <summary>
        /// 구매 가능 여부
        /// </summary>
        private bool IsPayAble(ShopItemData itemData)
        {
            if (itemData.isCash == false && itemData.price != 0)
            {
                int maxValue = itemData.variable.Value / itemData.price;
                if (maxValue <= 0)
                {
                    // TODO: 재화 부족해서 구매 불가능하다 알림
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region 결제
        private void Pay(UIShopSubTab tab, ShopSaveData.ShopCatalog shopCatalog, ShopSaveData.ShopItem shopItem, ShopItemData itemData, int buyCount = 1)
        {
            // 구매 횟수 제한이 있다면
            if (itemData.buyAbleCount > 0)
            {
                // 더 이상 아이템을 구매할 수 없다면
                int boughtCount = shopItem == null ? 0 : shopItem.boughtCount;
                if (boughtCount + buyCount > itemData.buyAbleCount)
                {
                    return;
                }
            }

            if (!CanGainShopItem(itemData, buyCount))
            {
                // TODO: 오류: 획득할 아이템 품목에 버그가 있습니다.
                return;
            }

            bool isSuccess = false;

            if (itemData.isCash)
            {
                isSuccess = CashPay(itemData);
            }
            else
            {
                isSuccess = VariablePay(itemData, buyCount);
            }

            if (isSuccess)
            {
                // 아이템 획득
                foreach (var gainItem in itemData.gainShopItemDatas)
                {
                    gainItem.GainShopItem(buyCount);
                }

                // 구매 횟수 제한이 있다면
                if (itemData.buyAbleCount > 0)
                {
                    shopCatalog.AddItem(itemData.itemName, buyCount);
                }

                tab.Select();

                _ = SaveManager.Instance.SaveData(SaveKey.Profile, SaveKey.Shop);
            }
        }

        private bool CanGainShopItem(ShopItemData itemData, int buyCount)
        {
            foreach (var gainItem in itemData.gainShopItemDatas)
            {
                if (!gainItem.CanGainShopItem(buyCount))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 현금 결제
        /// </summary>
        private bool CashPay(ShopItemData itemData)
        {
            // TODO: 구글 플레이스토어 결제는 추후 구현

            return true;
        }

        /// <summary>
        /// 인게임 재화 결제
        /// </summary>
        private bool VariablePay(ShopItemData itemData, int buyCount)
        {
            int payValue = itemData.price * buyCount;

            if (payValue > itemData.variable.Value) return false;

            itemData.variable.AddValue(-payValue);

            return true;
        }
        #endregion

        public void Hide()
        {
            VariableDisplayManager.Instance.HideAll();

            base.Hide(true);
        }
    }
}