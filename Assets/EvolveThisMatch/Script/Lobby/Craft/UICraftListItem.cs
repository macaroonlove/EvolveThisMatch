using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UICraftListItem : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            CraftBackground,
            CraftIcon,
            SelectDim,
        }
        enum Texts
        {
            CraftTime,
            CraftName,
            CraftWeight,
        }
        #endregion

        private Image _craftBackground;
        private Image _craftIcon;
        private Image _selectDim;
        private TextMeshProUGUI _craftTime;
        private TextMeshProUGUI _craftName;
        private TextMeshProUGUI _craftWeight;

        private UIRequiredItem[] _requiredItems;
        private UnityAction _action;

        internal int index { get; private set; }

        internal void Initialize(UnityAction action)
        {
            _action = action;

            _requiredItems = GetComponentsInChildren<UIRequiredItem>();

            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _craftBackground = GetImage((int)Images.CraftBackground);
            _craftIcon = GetImage((int)Images.CraftIcon);
            _selectDim = GetImage((int)Images.SelectDim);
            _craftTime = GetText((int)Texts.CraftTime);
            _craftName = GetText((int)Texts.CraftName);
            _craftWeight = GetText((int)Texts.CraftWeight);
        }

        internal void Show(CraftItemData itemData, int index)
        {
            this.index = index;

            int minutes = itemData.craftTime / 60;
            int seconds = itemData.craftTime % 60;

            _craftBackground.sprite = itemData.background;
            _craftIcon.sprite = itemData.variable.Icon;
            _craftTime.text = $"{minutes:D2}:{seconds:D2}";
            _craftName.text = itemData.variable.DisplayName;
            _craftWeight.text = $"{itemData.weight}kg";

            int requiredItemCount = itemData.requiredItems.Count;
            for (int i = 0; i < _requiredItems.Length; i++)
            {
                if (i < requiredItemCount)
                {
                    _requiredItems[i].Show(itemData.requiredItems[i]);
                }
                else
                {
                    _requiredItems[i].Hide(true);
                }
            }

            gameObject.SetActive(true);
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
            _action?.Invoke();

            _selectDim.enabled = true;
        }

        internal virtual void DeSelectItem()
        {
            _selectDim.enabled = false;
        }
    }
}