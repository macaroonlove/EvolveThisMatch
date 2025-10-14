using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UICraftListPanel : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        #endregion

        protected Transform _parent;
        protected List<UICraftListItem> _craftListItems;

        protected UnityAction<UICraftListItem> _action;

        private const int _itemCount = 10;

        internal void Initialize(UnityAction<UICraftListItem> action = null)
        {
            _action = action;

            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Content).transform;
        }

        protected void Start()
        {
            InitializeCraftListItem();

            _craftListItems[0].SelectItem();
        }

        internal void Show(DepartmentData template)
        {
            int count = template.CraftItems.Count;
            for (int i = 0; i < _itemCount; i++)
            {
                if (i < count)
                {
                    _craftListItems[i].Show(template.CraftItems[i], i);
                }
                else
                {
                    _craftListItems[i].Hide();
                }
            }
        }

        #region 리스트 아이템 생성
        private void InitializeCraftListItem()
        {
            _craftListItems = new List<UICraftListItem>(_itemCount);

            var craftListItem = GetComponentInChildren<UICraftListItem>();

            // 나머지 프리팹 인스턴스 생성
            for (int i = 0; i < _itemCount; i++)
            {
                var item = Instantiate(craftListItem.gameObject, _parent).GetComponent<UICraftListItem>();
                item.Initialize(() => ChangeCraft(item));
                _craftListItems.Add(item);
            }

            Destroy(craftListItem.gameObject);
        }
        #endregion

        private void ChangeCraft(UICraftListItem craftListItem)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _craftListItems) item.DeSelectItem();

            _action?.Invoke(craftListItem);
        }
    }
}