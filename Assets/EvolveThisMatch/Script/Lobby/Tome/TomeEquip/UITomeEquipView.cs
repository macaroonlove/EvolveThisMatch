using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UITomeEquipView : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            EquipButton,
        }
        enum Texts
        {
            EquipText,
        }
        #endregion

        private TextMeshProUGUI _equipText;
        private UITomeEquipItem[] _items;

        public event UnityAction<TomeTemplate, ItemSaveData.Tome, int> onSelected;

        public void Initialize(UITomeListCanvas listCanvas)
        {
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _equipText = GetText((int)Texts.EquipText);

            var model = new UITomeEquipModel();
            var presenter = new UITomeEquipPresenter(this, listCanvas, model);

            GetButton((int)Buttons.EquipButton).onClick.AddListener(presenter.Equip);
        }

        public void InitializeTomeEquipItem(UnityAction<int> onSelect)
        {
            _items = GetComponentsInChildren<UITomeEquipItem>();
            for (int i = 0; i < _items.Length; i++)
            {
                int index = i;
                _items[i].Initialize(() => onSelect?.Invoke(index));
            }
        }

        public void Render(string text)
        {
            _equipText.text = text;
        }

        public void RenderItem(int index, TomeEquipItemViewState state)
        {
            _items[index].Render(state);
        }

        public void SelectItem(int index)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].Select(i == index);
            }
        }

        public void OnSelected(TomeTemplate template, ItemSaveData.Tome owned, int index)
        {
            onSelected?.Invoke(template, owned, index);
        }
    }
}