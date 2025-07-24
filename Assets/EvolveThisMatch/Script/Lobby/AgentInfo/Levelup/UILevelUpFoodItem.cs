using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UILevelUpFoodItem : UIBase, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
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
            _variable.AddValue(-count);
        }

        internal void ResetStock()
        {
            count = _variable.Value;
            _countText.text = count.ToString();
        }

        internal void ResetEat()
        {
            count = 0;
            _countText.text = "0";
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}