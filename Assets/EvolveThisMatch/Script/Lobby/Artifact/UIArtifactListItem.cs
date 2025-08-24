using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIArtifactListItem : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Level,
            DisplayName,
            CounterText,
        }
        enum Images
        {
            Icon,
            CounterImage,
            SelectDim,
        }
        #endregion

        private TextMeshProUGUI _level;
        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _counterText;
        private Image _icon;
        private Image _counterImage;
        private Image _selectDim;

        internal ArtifactTemplate template { get; private set; }
        internal ItemSaveData.Artifact owned { get; private set; }

        private UnityAction<ArtifactTemplate, ItemSaveData.Artifact> _action;

        internal virtual void Initialize(UnityAction<ArtifactTemplate, ItemSaveData.Artifact> action = null)
        {
            _action = action;

            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _level = GetText((int)Texts.Level);
            _displayName = GetText((int)Texts.DisplayName);
            _counterText = GetText((int)Texts.CounterText);
            _icon = GetImage((int)Images.Icon);
            _counterImage = GetImage((int)Images.CounterImage);
            _selectDim = GetImage((int)Images.SelectDim);
        }

        internal virtual void Show(ArtifactTemplate template, ItemSaveData.Artifact owned)
        {
            if (owned == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            this.template = template;
            this.owned = owned;

            _icon.sprite = template.sprite;
            _displayName.text = template.displayName;
            _level.text = $"Lv. {owned.level}";

            int artifactCount = owned.count;
            int maxArtifactCount = SaveManager.Instance.itemData.GetMaxArtifactCountByLevel(owned.level);

            if (maxArtifactCount == -1)
            {
                _counterText.text = $"{artifactCount}";
                _counterImage.fillAmount = 1;
            }
            else
            {
                _counterText.text = $"{artifactCount}/{maxArtifactCount}";
                _counterImage.fillAmount = artifactCount / maxArtifactCount;
            }
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem();
        }

        internal virtual void SelectItem()
        {
            _action?.Invoke(template, owned);

            _selectDim.enabled = true;
        }

        internal virtual void DeSelectItem()
        {
            _selectDim.enabled = false;
        }
    }
}