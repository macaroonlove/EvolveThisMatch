using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UITomeEquipCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            EquipButton,
        }
        enum Texts
        {
            EquipText,
        }
        #endregion

        enum EquipState
        {
            Rent,
            Return,
        }

        private UITomeEquipItem[] _tomeEquipItems;
        protected List<TomeTemplate> _tomeTemplates;

        private TextMeshProUGUI _equipText;

        private EquipState _equipState;
        private UITomeListItem _selectedTomeListItem;
        private UITomeEquipItem _selectedTomeEquipItem;

        private UnityAction<UITomeEquipItem> _action;
        private UnityAction<int> _rentEquip;
        private UnityAction<int> _returnEquip;

        internal virtual void Initialize(UnityAction<UITomeEquipItem> action, UnityAction<int> rentEquip, UnityAction<int> returnEquip)
        {
            _action = action;
            _rentEquip = rentEquip;
            _returnEquip = returnEquip;

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _equipText = GetText((int)Texts.EquipText);
            GetButton((int)Buttons.EquipButton).onClick.AddListener(Equip);
        }

        internal void InitializeItem()
        {
            InitializeTomeEquipItem();

            _tomeEquipItems[0].SelectItem();
        }

        #region 장착 아이템 초기화
        private void InitializeTomeEquipItem()
        {
            _tomeEquipItems = GetComponentsInChildren<UITomeEquipItem>();
            _tomeTemplates = GameDataManager.Instance.tomeTemplates.ToList();

            for (int i = 0; i < _tomeEquipItems.Length; i++) _tomeEquipItems[i].Initialize(i, ChangeTome);

            RegistTomeEquipItem();
        }

        private void ChangeTome(UITomeEquipItem item)
        {
            DeSelectAllItem();

            if (item.template != null)
            {
                _equipText.text = "반납하기";
                _equipState = EquipState.Return;

                _action?.Invoke(item);
            }
            else
            {
                _equipText.text = "대여하기";
                _equipState = EquipState.Rent;
            }

            _selectedTomeEquipItem = item;
        }

        internal void DeSelectAllItem()
        {
            // 모든 아이템 선택 취소
            foreach (var item in _tomeEquipItems) item.DeSelectItem();
        }

        internal void RegistTomeEquipItem()
        {
            var itemData = SaveManager.Instance.itemData;
            var ownedTomes = itemData.ownedTomes;
            var equipTomes = itemData.equipTomes;
            int count = _tomeEquipItems.Length;

            // 보유한 고서의 아이디
            var ownedTomeDic = ownedTomes.ToDictionary(a => a.id);

            for (int i = 0; i < count; i++)
            {
                // 장착한 상태라면
                if (equipTomes[i] != -1 && ownedTomeDic.TryGetValue(equipTomes[i], out var owned))
                {
                    var template = _tomeTemplates[equipTomes[i]];
                    int index = ownedTomes.FindIndex(t => t.id == equipTomes[i]);

                    _tomeEquipItems[i].Show(template, owned, index);
                }
                // 비어있는 상태라면
                else
                {
                    _tomeEquipItems[i].Hide();
                }
            }
        }
        #endregion

        internal void SelectTomeListItem(UITomeListItem item)
        {
            _equipText.text = "대여하기";
            _equipState = EquipState.Rent;

            _selectedTomeListItem = item;
        }

        private async void Equip()
        {
            if (_equipState == EquipState.Rent)
            {
                if (_selectedTomeEquipItem != null)
                {
                    int listItemIndex = _selectedTomeEquipItem.listItemIndex;

                    _returnEquip?.Invoke(listItemIndex);
                }

                var template = _selectedTomeListItem.template;
                var owned = _selectedTomeListItem.owned;
                int index = _selectedTomeListItem.index;

                _selectedTomeEquipItem.Show(template, owned, index);
                _selectedTomeListItem.Hide();
                _rentEquip?.Invoke(index);

                _equipText.text = "반납하기";
                _equipState = EquipState.Return;

                SaveManager.Instance.itemData.EquipTome(template.id, _selectedTomeEquipItem.index);
            }
            else if (_equipState == EquipState.Return)
            {
                int listItemIndex = _selectedTomeEquipItem.listItemIndex;
                if (listItemIndex != -1)
                {
                    _returnEquip?.Invoke(listItemIndex);
                }
                _selectedTomeEquipItem.Hide();

                _equipText.text = "대여하기";
                _equipState = EquipState.Rent;

                SaveManager.Instance.itemData.EquipTome(-1, _selectedTomeEquipItem.index);
            }

            await SaveManager.Instance.Save_ProfileData();
        }
    }
}