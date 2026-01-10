using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UICraftListView : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        #endregion

        [SerializeField] private GameObject _prefab;
        private List<UICraftListItem> _craftListItems;

        public event UnityAction<int> onSelectCraftItem;

        private const int _itemCount = 10;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));

            InitializeCraftListItem();
        }

        #region 리스트 아이템 생성
        private void InitializeCraftListItem()
        {
            _craftListItems = new List<UICraftListItem>(_itemCount);

            var parent = GetObject((int)Objects.Content).transform;

            for (int i = 0; i < _itemCount; i++)
            {
                int index = i;
                var item = Instantiate(_prefab, parent).GetComponent<UICraftListItem>();
                item.Initialize(() => SelectCraftItem(index));
                _craftListItems.Add(item);
            }
        }
        #endregion

        internal void Show(DepartmentData titleData)
        {
            int count = titleData.CraftItems.Count;
            for (int i = 0; i < _itemCount; i++)
            {
                if (i < count)
                {
                    _craftListItems[i].Show(titleData.CraftItems[i]);
                }
                else
                {
                    _craftListItems[i].Hide();
                }
            }

            _craftListItems[0].SelectItem();
        }

        private void SelectCraftItem(int index)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _craftListItems) item.DeSelectItem();

            onSelectCraftItem?.Invoke(index);
        }
    }
}