using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITomeListItem : UIBase, IPointerClickHandler
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

        internal int index { get; private set; }
        internal bool isEquip { get; private set; }
        internal TomeTemplate template { get; private set; }
        internal ProfileSaveData.Tome owned { get; private set; }

        private UnityAction<UITomeListItem> _action;

        internal virtual void Initialize(int index, UnityAction<UITomeListItem> action = null)
        {
            this.index = index;
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

        internal virtual void Show(TomeTemplate template, ProfileSaveData.Tome owned)
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

            int tomeCount = owned.count;
            int maxTomeCount = GameDataManager.Instance.profileSaveData.GetMaxTomeCountByLevel(owned.level);

            if (maxTomeCount == -1)
            {
                _counterText.text = $"{tomeCount}";
                _counterImage.fillAmount = 1;
            }
            else
            {
                _counterText.text = $"{tomeCount}/{maxTomeCount}";
                _counterImage.fillAmount = tomeCount / maxTomeCount;
            }
        }

        public override void Show(bool isForce = false)
        {
            gameObject.SetActive(true);
            isEquip = false;
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
            isEquip = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem();
        }

        internal virtual void SelectItem()
        {
            _action?.Invoke(this);

            _selectDim.enabled = true;
        }

        internal virtual void DeSelectItem()
        {
            _selectDim.enabled = false;
        }
    }
}