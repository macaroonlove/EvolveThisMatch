using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public abstract class UIEngraveButton : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            PayText,
            LevelText,
        }
        #endregion

        protected TextMeshProUGUI _payText;
        protected TextMeshProUGUI _levelText;
        protected Color _originTextColor;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _payText = GetText((int)Texts.PayText);
            _levelText = GetText((int)Texts.LevelText);
            _originTextColor = _payText.color;

            GetComponent<Button>().onClick.AddListener(Engrave);
        }

        internal abstract void InitializeBattle(UIEngraveCanvas engraveCanvas);

        internal abstract void DeinitializeBattle();

        protected abstract void Engrave();
    }
}