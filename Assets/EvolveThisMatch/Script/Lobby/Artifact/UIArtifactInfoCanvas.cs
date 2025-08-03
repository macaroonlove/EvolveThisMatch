using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

        private UnityAction _action;

        internal void Initialize(UnityAction action = null)
        {
            _action = action;

            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _displayName = GetText((int)Texts.DisplayName);
            _description = GetText((int)Texts.Description);
            _icon = GetImage((int)Images.Icon);
        }

        internal void Show(ArtifactTemplate template, ProfileSaveData.Artifact owned)
        {
            if (template == null) return;

            _displayName.text = template.displayName;
            _description.text = template.description;
            _icon.sprite = template.sprite;

            //owned.level
        }
    }
}