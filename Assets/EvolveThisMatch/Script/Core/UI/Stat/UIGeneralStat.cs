using Cysharp.Threading.Tasks;
using FrameWork.Tooltip;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [RequireComponent(typeof(TooltipTrigger))]
    public abstract class UIGeneralStat : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Value
        }
        #endregion

        private TextMeshProUGUI _valueText;

        protected override async void Awake()
        {
            base.Awake();

            BindText(typeof(Texts));
            _valueText = GetText((int)Texts.Value);

            await UniTask.Yield();

            var tooltip = GetComponent<TooltipTrigger>();
            tooltip.SetText("Description", GetDiscription());
        }

        protected abstract string GetDiscription();

        protected void Apply(float value, int decimalPoint = 0)
        {
            if (_valueText != null)
            {
                _valueText.text = value.ToString($"F{decimalPoint}");
            }
            gameObject.SetActive(true);
        }

        public void Deinitialize()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            _valueText.text = "NULL";
        }
    }
}