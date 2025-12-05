using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UIAcquirePopup : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            Button,
        }
        enum Objects
        {
            Content,
            Popup,
        }
        #endregion

        [SerializeField] private GameObject _prefab;

        private Transform _parent;
        private RectTransform _rect;
        private ScrollRect _scrollRect;
        private UIParticle _showVFX;

        private List<UIAcquireItem> _items = new List<UIAcquireItem>();

        public event UnityAction OnResult;

        protected override void Initialize()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
            _showVFX = GetComponentInChildren<UIParticle>();
            
            BindObject(typeof(Objects));
            BindButton(typeof(Buttons));
            
            _rect = GetObject((int)Objects.Popup).transform as RectTransform;
            _parent = GetObject((int)Objects.Content).transform;
            GetButton((int)Buttons.Button).onClick.AddListener(() => Hide(true));
        }

        public void Show(List<AcquireItem> acquireItems)
        {
            base.Show(false);
            _showVFX.Play();
            _rect.localScale = new Vector3(0, 1, 1);

            ShowAnimation(acquireItems);
        }

        private async void ShowAnimation(List<AcquireItem> acquireItems)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.3f));

            _rect.DOScaleX(1, 0.25f).OnComplete(async () =>
            {
                for (int i = 0; i < acquireItems.Count; i++)
                {
                    var item = GetItem(i);
                    item.Show(acquireItems[i]);
                }

                for (int i = acquireItems.Count; i < _items.Count; i++)
                {
                    _items[i].Hide(true);
                }

                await UniTask.NextFrame();
                _scrollRect.horizontalNormalizedPosition = 0;
            });
        }

        public UIAcquireItem GetItem(int index)
        {
            if (index < _items.Count)
            {
                return _items[index];
            }

            var instance = Instantiate(_prefab, _parent);
            var newItem = instance.GetComponent<UIAcquireItem>();
            _items.Add(newItem);

            return newItem;
        }

        public override void Hide(bool isForce = false)
        {
            base.Hide(isForce);

            foreach (var item in _items)
            {
                item.Hide(true);
            }

            OnResult?.Invoke();
            OnResult = null;
        }
    }

    public struct AcquireItem
    {
        public Sprite icon;
        public int count;
        public string displayName;

        public AcquireItem(Sprite icon, int count, string displayName)
        {
            this.icon = icon;
            this.count = count;
            this.displayName = displayName;
        }
    }
}