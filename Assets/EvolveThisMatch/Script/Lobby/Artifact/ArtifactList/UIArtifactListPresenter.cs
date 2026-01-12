namespace EvolveThisMatch.Lobby
{
    public sealed class UIArtifactListPresenter
    {
        private readonly UIArtifactListCanvas _view;
        private readonly UIArtifactListModel _model;

        public UIArtifactListPresenter(UIArtifactListCanvas view, UIArtifactListModel model)
        {
            _view = view;
            _model = model;

            _view.InitializeArtifactListItem(_model.Count, OnSelect);
            _view.onRefresh += Refresh;

            Refresh();
            OnSelect(0);
        }

        private void Refresh()
        {
            for (int i = 0; i < _model.Count; i++)
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
    }
}