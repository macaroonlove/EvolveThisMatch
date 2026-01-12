using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIPopup;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITomeEquipPresenter
    {
        enum EquipState
        {
            Rent,
            Return,
        }

        private readonly UITomeEquipView _view;
        private readonly UITomeListCanvas _listView;
        private readonly UITomeEquipModel _model;

        private EquipState _equipState;
        private TomeEquipData _selectedData;
        private int _index;

        public UITomeEquipPresenter(UITomeEquipView view, UITomeListCanvas listView, UITomeEquipModel model)
        {
            _view = view;
            _listView = listView;
            _model = model;

            _view.InitializeTomeEquipItem(OnSelect);
            _model.InitializeEquipItem();
            Refresh();

            _listView.onSelected += OnSelectListItem;
            _view.onSelected += OnSelectEquipItem;

            OnSelect(0);
        }

        private void Refresh()
        {
            for (int i = 0; i < _model.count; i++)
            {
                var template = _model.GetEquipData(i).template;
                if (template != null)
                {
                    _listView.HideItem(template.id);
                }
                
                _view.RenderItem(i, _model.BuildState(i));
            }
        }

        private void OnSelect(int index)
        {
            _view.SelectItem(index);

            var data = _model.GetEquipData(index);

            _view.OnSelected(data.template, data.owned, index);
        }

        private void OnSelectListItem(TomeTemplate template, ItemSaveData.Tome owned)
        {
            if (template != null)
            {
                _equipState = EquipState.Rent;
                _view.Render("대여하기");

                _selectedData = new TomeEquipData(template, owned);
            }
        }

        private void OnSelectEquipItem(TomeTemplate template, ItemSaveData.Tome owned, int index)
        {
            if (template != null)
            {
                _equipState = EquipState.Return;
                _view.Render("반납하기");

                _selectedData = new TomeEquipData(template, owned);
            }

            _index = index;
        }

        public void Equip()
        {
            if (_selectedData.template == null)
            {
                UIPopupManager.Instance.ShowNotificationPopup("고서를 선택해 주세요");
                return;
            }

            switch (_equipState)
            {
                case EquipState.Rent:
                    Rent();
                    break;
                case EquipState.Return:
                    Return();
                    break;
            }
        }

        private void Rent()
        {
            _equipState = EquipState.Return;
            _view.Render("반납하기");

            int id = _model.Equip(_index, _selectedData);
            if (id != -1) _listView.ShowItem(id);

            _view.RenderItem(_index, _model.BuildState(_index));
            _listView.HideItem(_selectedData.template.id);

            SaveManager.Instance.formationData.EquipTome(_selectedData.template.id, _index);
            SaveManager.Instance.Save_FormationData();
        }

        private void Return()
        {
            _equipState = EquipState.Rent;
            _view.Render("대여하기");

            _selectedData = default;

            int id = _model.Equip(_index, _selectedData);
            if (id != -1) _listView.ShowItem(id);

            _view.RenderItem(_index, default);

            SaveManager.Instance.formationData.EquipTome(-1, _index);
            SaveManager.Instance.Save_FormationData();
        }
    }

    public struct TomeEquipData
    {
        public TomeTemplate template;
        public ItemSaveData.Tome owned;

        public TomeEquipData(TomeTemplate template, ItemSaveData.Tome owned)
        {
            this.template = template;
            this.owned = owned;
        }
    }
}