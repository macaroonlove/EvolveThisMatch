using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EvolveThisMatch.Lobby
{
    public class UILevelUpFoodSelectItem : UIBase, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private enum EFoodItemType
        {
            /// <summary>
            /// 보유 음식
            /// </summary>
            Stock,
            /// <summary>
            /// 제단에 올린 음식
            /// </summary>
            Eat,
        }

        #region 바인딩
        enum Images
        {
            Icon,
        }
        enum Texts
        {
            Count,
        }
        #endregion

        [SerializeField] private ObscuredIntVariable _variable;
        [SerializeField] private EFoodItemType _itemType;

        private TextMeshProUGUI _countText;

        private bool _isPress;
        private float _pressTime;

        private UnityAction _action;
        internal int count { get; private set; }

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            GetImage((int)Images.Icon).sprite = _variable.Icon;

            _countText = GetText((int)Texts.Count);
            _countText.text = "0";
        }

        internal void InitializeItem(UnityAction action)
        {
            _action = action;
        }

        #region 이벤트
        public void OnPointerClick(PointerEventData eventData)
        {
            Transfer();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPress = true;
            _pressTime = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPress = false;
            _pressTime = 0f;
        }

        private void Update()
        {
            if (_isPress)
            {
                _pressTime += Time.deltaTime;

                // 0.3초 이후에 매 프레임 인식
                if (_pressTime >= 0.3f)
                {
                    Transfer();
                }
            }
        }

        private void Transfer()
        {
            if (count > 0)
            {
                _action?.Invoke();
            }
        }
        #endregion

        internal void Increment()
        {
            count++;
            _countText.text = count.ToString();
        }

        internal void Decrement()
        {
            count--;
            _countText.text = count.ToString();
        }

        internal void PayFood()
        {
            if (_itemType != EFoodItemType.Eat) return;

            _variable.AddValue(-count);
        }

        internal void ResetItem()
        {
            if (_itemType == EFoodItemType.Stock)
            {
                count = _variable.Value;
            }
            else if (_itemType == EFoodItemType.Eat)
            {
                count = 0;
            }

            _countText.text = count.ToString();
        }
    }
}