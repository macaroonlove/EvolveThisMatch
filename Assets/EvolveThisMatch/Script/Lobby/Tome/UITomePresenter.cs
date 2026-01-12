using EvolveThisMatch.Core;
using EvolveThisMatch.Save;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITomePresenter
    {
        private readonly UITomeInfoView _infoView;
        private readonly UITomeModel _model;

        public UITomePresenter(UITomeListCanvas listCanvas, UITomeInfoView infoView, UITomeEquipView equipView, UITomeModel model)
        {
            _infoView = infoView;
            _model = model;

            listCanvas.onSelected += SelectListItem;
            if (equipView != null)
            {
                equipView.Initialize(listCanvas);
                equipView.onSelected += SelectEquipItem;
            }
        }

        private void SelectListItem(TomeTemplate template, ItemSaveData.Tome owned)
        {
            var state = _model.BuildInfoViewState(template, owned);

            _infoView.Show(state);
        }

        private void SelectEquipItem(TomeTemplate template, ItemSaveData.Tome owned, int index)
        {
            if (template == null || owned == null) return;

            var state = _model.BuildInfoViewState(template, owned);

            _infoView.Show(state);
        }
    }
}