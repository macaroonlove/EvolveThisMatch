using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIShopCanvas : UIBase
    {
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

        [Header("상점 데이터")]
        [SerializeField] private ShopDataTemplateLibrary _shopDataTemplateLibrary;

        private List<UIShopSubTab> _shopSubTabList = new List<UIShopSubTab>();
        private List<UIDefaultShopItem> _defaultShopItems = new List<UIDefaultShopItem>();
        private List<UIPackageShopItem> _packageShopItems = new List<UIPackageShopItem>();
        private UIShopMainTab _firstMainTab;
        private UIShopMainTab _currentTab;

        protected override void Initialize()
        {
            InitializeItem();
            InitializeTab();
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
        }

        private void SelectSubTab(UIShopSubTab tab)
        {
            foreach (var subTab in _shopSubTabList) subTab.UnSelect();

            foreach (var item in _packageShopItems) item.Hide(true);
            foreach (var item in _defaultShopItems) item.Hide(true);

            var items = tab.data.shopItems;

            int pIndex = 0;
            int dIndex = 0;
            foreach (var item in items)
            {
                if (item.isPackage)
                {
                    _packageShopItems[pIndex].Show(item);
                    pIndex++;
                }
                else
                {
                    _defaultShopItems[dIndex].Show(item);
                    dIndex++;
                }
            }
        }
    }
}