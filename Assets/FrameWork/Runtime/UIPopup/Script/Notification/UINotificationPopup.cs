using Cysharp.Threading.Tasks;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UIPopup
{
    public class UINotificationPopup : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Layout,
        }
        #endregion

        [SerializeField] private GameObject _prefab;

        private Transform _parent;

        private LinkedList<UINotificationItem> _activeItems;
        private Stack<UINotificationItem> _items;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Layout).transform;

            InitializeItems();
        }

        private void InitializeItems()
        {
            _activeItems = new LinkedList<UINotificationItem>();
            _items = new Stack<UINotificationItem>(3);

            for (int i = 0; i < 3; i++)
            {
                var instance = Instantiate(_prefab, _parent);
                var item = instance.GetComponent<UINotificationItem>();
                item.InitializeCTS();
                item.Hide(true);
                _items.Push(item);
            }
        }

        public void Show(string context)
        {
            // 보여줄 수 있는 팝업이 없다면 반환
            if (_items.Count == 0) return;

            ShowNewPopup(context);

            ToastAnimation();
        }

        private async void ShowNewPopup(string context)
        {
            // 새 팝업 가져오기
            var newItem = _items.Pop();
            newItem.Show(context);
            _activeItems.AddFirst(newItem);

            newItem.CancelDelay();

            // 2초 대기
            try
            {
                await newItem.Delay(2f);
            }
            catch
            {
                return;
            }

            ReturnToPool(newItem);
        }

        /// <summary>
        /// 팝업 토스트 애니메이션
        /// </summary>
        private void ToastAnimation()
        {
            int index = 0;

            foreach (var item in _activeItems)
            {
                if (index >= 2)
                {
                    item.Hide(false);
                    item.ToastMove(-120 * index - 20, () => ReturnToPool(item));
                }
                else
                {
                    item.ToastMove(-120 * index - 20);
                }

                index++;
            }
        }

        private void ReturnToPool(UINotificationItem item)
        {
            if (!_activeItems.Contains(item)) return;

            item.CancelDelay();

            _activeItems.Remove(item);
            _items.Push(item);

            item.Hide(false);
        }
    }
}