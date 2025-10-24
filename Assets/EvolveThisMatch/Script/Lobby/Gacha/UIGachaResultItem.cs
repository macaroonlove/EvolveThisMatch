using FrameWork.UIBinding;
using EvolveThisMatch.Core;
using TMPro;
using UnityEngine.UI;
using FrameWork;
using ScriptableObjectArchitecture;
using UnityEngine;

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

        [SerializeField] private Sprite _artifactBackground;
        [SerializeField] private Sprite _tomeBackground;

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
                _background.sprite = agentTemplate.rarity.agentInfoSprite;
                _displayName.text = agentTemplate.displayName;
            }
            else if (type == "Artifact")
            {
                var artifactTemplate = await AddressableAssetManager.Instance.GetScriptableObject<ArtifactTemplate>(result);

                _icon.sprite = artifactTemplate.sprite;
                _background.sprite = _artifactBackground;
                _displayName.text = artifactTemplate.displayName;
            }
            else if (type == "Tome")
            {
                var tomeTemplate = await AddressableAssetManager.Instance.GetScriptableObject<TomeTemplate>(result);

                _icon.sprite = tomeTemplate.sprite;
                _background.sprite = _tomeBackground;
                _displayName.text = tomeTemplate.displayName;
            }
            else
            {
                int value = int.Parse(parts[1]);

                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(result);

                _icon.sprite = variable.Icon;
                _background.sprite = variable.IconBG;
                _displayName.text = $"{variable.DisplayName} x{value}";
            }
        }
    }
}