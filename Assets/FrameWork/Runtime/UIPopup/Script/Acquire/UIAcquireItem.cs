using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UIAcquireItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Count,
            DisplayName,
        }
        enum Images
        {
            Icon,
        }
        #endregion

        private TextMeshProUGUI _count;
        private TextMeshProUGUI _displayName;
        private Image _icon;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _count = GetText((int)Texts.Count);
            _displayName = GetText((int)Texts.DisplayName);
            _icon = GetImage((int)Images.Icon);
        }

        internal void Show(AcquireItem item)
        {
            _count.text = item.count.ToString();
            _displayName.text = item.displayName;
            _icon.sprite = item.icon;

            base.Show(true);
        }
    }
}