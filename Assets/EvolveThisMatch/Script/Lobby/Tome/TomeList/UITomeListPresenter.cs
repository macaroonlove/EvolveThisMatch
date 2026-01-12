namespace EvolveThisMatch.Lobby
{
    public sealed class UITomeListPresenter
    {
        private readonly UITomeListCanvas _view;
        private readonly UITomeListModel _model;

        public UITomeListPresenter(UITomeListCanvas view, UITomeListModel model)
        {
            _view = view;
            _model = model;

            _view.InitializeTomeListItem(_model.count, OnSelect);
            _view.onRefresh += Refresh;

            Refresh();
            OnSelect(0);
        }

        private void Refresh()
        {
            for (int i = 0; i < _model.count; i++)
            {
                var state = _model.BuildState(i);
                _view.RenderItem(i, state);
            }
        }

        private void OnSelect(int index)
        {
            _view.SelectItem(index);

            var template = _model.GetTemplate(index);
            var owned = _model.GetOwned(template);

            _view.OnSelected(template, owned);
        }

        public void Show(int id)
        {
            _view.RenderItem(id, _model.BuildState(id));
            OnSelect(id);
        }

        public void Hide(int id)
        {
            _view.RenderItem(id, default);
        }
    }
}