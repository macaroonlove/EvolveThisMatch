using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using System.Text;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UISynergyItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            SynergyIcon,
        }
        enum Texts
        {
            SynergyText,
            SynergyDesc,
        }
        #endregion

        private TextMeshProUGUI _synergyText;
        private TextMeshProUGUI _synergyDesc;
        private Image _synergyIcon;

        private StringBuilder _description = new StringBuilder(128);

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _synergyIcon = GetImage((int)Images.SynergyIcon);
            _synergyText = GetText((int)Texts.SynergyText);
            _synergyDesc = GetText((int)Texts.SynergyDesc);
        }

        internal void Show(SynergyTemplate template)
        {
            if (template == null) return;

            _description.Clear();
            _description.Append($"{template.description}");
            _description.Append($"\n<size=10>\n</size>");

            foreach (var effect in template.synergyDatas)
            {
                _description.Append($"({effect.count}) {effect.buff.description}\n");
            }

            _synergyText.text = template.displayName;
            _synergyIcon.sprite = template.icon;
            _synergyDesc.text = _description.ToString();
        }
    }
}