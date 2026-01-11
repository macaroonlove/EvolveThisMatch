using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIArtifactInfoView : UIBase
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

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _displayName = GetText((int)Texts.DisplayName);
            _description = GetText((int)Texts.Description);
            _icon = GetImage((int)Images.Icon);
        }

        internal void Show(ArtifactInfoViewState state)
        {
            _displayName.text = state.displayName;
            _description.text = state.description;
            _icon.sprite = state.icon;
        }
    }

    public readonly struct ArtifactInfoViewState
    {
        public readonly string displayName;
        public readonly string description;
        public readonly Sprite icon;

        public ArtifactInfoViewState(string displayName, string description, Sprite icon)
        {
            this.displayName = displayName;
            this.description = description;
            this.icon = icon;
        }
    }
}