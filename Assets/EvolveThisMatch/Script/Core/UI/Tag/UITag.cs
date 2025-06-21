using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public abstract class UITag : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
        }
        enum Texts
        {
            Text,
        }
        #endregion

        protected Image _background;
        protected TextMeshProUGUI _text;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _background = GetImage((int)Images.Background);
            _text = GetText((int)Texts.Text);
        }
    }
}