using FrameWork;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UILootItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Amount,
        }
        enum Images
        {
            Background,
            Icon,
        }
        #endregion

        private TextMeshProUGUI _amount;
        private Image _background;
        private Image _icon;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _amount = GetText((int)Texts.Amount);
            _background = GetImage((int)Images.Background);
            _icon = GetImage((int)Images.Icon);
        }

        internal void Show(ObscuredIntVariable variable, int amount)
        {
            _amount.text = amount.Format(4, 2);
            _background.sprite = variable.IconBG;
            _icon.sprite = variable.Icon;
        }
    }
}