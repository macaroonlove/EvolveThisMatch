using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UI;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIShopCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
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

        protected override void Initialize()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _packagePayPanel = GetComponentInChildren<UIPackagePayPanel>();
            _defaultPayPanel = GetComponentInChildren<UIDefaultPayPanel>();

            InitializeItem();
            InitializeTab();

            BindButton(typeof(Buttons));

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

        private void SelectSubTab(UIShopSubTab tab)
        {
            foreach (var subTab in _shopSubTabList) subTab.UnSelect();

            foreach (var item in _packageShopItems) item.Hide(true);
            foreach (var item in _defaultShopItems) item.Hide(true);
            foreach (var item in _shopGainItems.Keys) _poolSystem.DeSpawn(item);

            var items = tab.data.shopItems;

            int pIndex = 0;
            int dIndex = 0;
            foreach (var item in items)
            {
                if (item.isPackage)
                {
                    var gainItemList = GetGainItem(item.gainShopItemDatas.Count);

                    _packageShopItems[pIndex].Show(item, gainItemList, SelectPackage);
                    pIndex++;
                }
                else
                {
                    _defaultShopItems[dIndex].Show(item, SelectDefault);
                    dIndex++;
                }
            }
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

        #region 아이템 선택
        private void SelectPackage(ShopItemData itemData)
        {
            if (!IsPayAble(itemData)) return;

            if (itemData.isPanel)
            {
                _packagePayPanel.Show(itemData, () => Pay(itemData));
            }
            else
            {
                Pay(itemData);
            }
        }

        private void SelectDefault(ShopItemData itemData)
        {
            if (!IsPayAble(itemData)) return;

            if (itemData.isPanel)
            {
                _defaultPayPanel.Show(itemData, (buyCount) => Pay(itemData, buyCount));
            }
            else
            {
                Pay(itemData);
            }
        }

        /// <summary>
        /// 구매 가능 여부
        /// </summary>
        private bool IsPayAble(ShopItemData itemData)
        {
            if (itemData.isCash == false && itemData.needCount != 0)
            {
                int maxValue = itemData.variable.Value / itemData.needCount;
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
        private async void Pay(ShopItemData itemData, int buyCount = 1)
        {
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
                foreach (var gainItem in itemData.gainShopItemDatas)
                {
                    gainItem.GainShopItem(buyCount);
                }

                await SaveManager.Instance.Save_ProfileData();
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
            int payValue = itemData.needCount * buyCount;

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