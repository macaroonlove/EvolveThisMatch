using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UISynergyItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            SynergyItem,
            Icon,
        }
        enum Texts
        {
            Name,
        }
        #endregion

        private TextMeshProUGUI _name;
        private Image _synergyItem;
        private Image _icon;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _synergyItem = GetImage((int)Images.SynergyItem);
            _icon = GetImage((int)Images.Icon);
            _name = GetText((int)Texts.Name);
        }

        internal void Show(SynergyTemplate template)
        {
            _name.text = template.displayName;
            _icon.sprite = template.icon;

            base.Show(true);
        }
    }
}
