using FrameWork.UIBinding;
using EvolveThisMatch.Core;
using TMPro;
using UnityEngine.UI;
using FrameWork;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaResultItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            GachaResultItem,
            Icon,
        }
        enum Texts
        {
            DisplayName,
        }
        #endregion

        private Image _background;
        private Image _icon;
        private TextMeshProUGUI _displayName;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _background = GetImage((int)Images.GachaResultItem);
            _icon = GetImage((int)Images.Icon);
            _displayName = GetText((int)Texts.DisplayName);
        }

        internal async void Show(string result)
        {
            var parts = result.Split('_');
            string type = parts[0];

            if (type == "Agent")
            {
                var agentTemplate = await AddressableAssetManager.Instance.GetScriptableObject<AgentTemplate>(result);

                _icon.sprite = agentTemplate.sprite;
                _displayName.text = agentTemplate.displayName;
            }
            else if (type == "Artifact")
            {
                var artifactTemplate = await AddressableAssetManager.Instance.GetScriptableObject<ArtifactTemplate>(result);

                _icon.sprite = artifactTemplate.sprite;
                _displayName.text = artifactTemplate.displayName;
            }
            else if (type == "Tome")
            {
                var tomeTemplate = await AddressableAssetManager.Instance.GetScriptableObject<TomeTemplate>(result);

                _icon.sprite = tomeTemplate.sprite;
                _displayName.text = tomeTemplate.displayName;
            }
        }
    }
}