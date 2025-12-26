using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIArtifactInfoCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            DisplayName,
            Description,
        }
        enum Images
        {
            Icon,
        }
        #endregion

        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _description;
        private Image _icon;

        private EffectContext _effectContext;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _displayName = GetText((int)Texts.DisplayName);
            _description = GetText((int)Texts.Description);
            _icon = GetImage((int)Images.Icon);

            _effectContext = new EffectContext();
        }

        internal void Show(ArtifactTemplate template, ItemSaveData.Artifact owned)
        {
            if (template == null) return;

            _displayName.text = template.displayName;
            _icon.sprite = template.sprite;

            _effectContext.artifactSaveData = owned;
            _description.text = template.description.Replace("{value}", $"{template.GetValue("value", _effectContext)}");
        }
    }
}