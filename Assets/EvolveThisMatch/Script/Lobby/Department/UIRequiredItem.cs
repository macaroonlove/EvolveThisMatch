using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIRequiredItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Icon,
        }
        enum Texts
        {
            NeedCount,
        }
        #endregion

        private Image _icon;
        private TextMeshProUGUI _needCount;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _icon = GetImage((int)Images.Icon);
            _needCount = GetText((int)Texts.NeedCount);
        }

        internal void Show(CraftItemData.RequiredItem requiredItem)
        {
            _icon.sprite = requiredItem.item.Icon;
            _needCount.text = requiredItem.amount.ToString();

            base.Show(true);
        }
    }
}
