using EvolveThisMatch.Save;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITierUpController
    {
        private readonly UITierUpView _view;
        private readonly UITierUpModel _model;

        private UnityAction _reShow;

        public UITierUpController(UITierUpView view, UITierUpModel model)
        {
            _view = view;
            _model = model;
        }

        public void Show(AgentSaveData.Agent owned, UnityAction reShow)
        {
            _reShow = reShow;
            _model.Bind(owned);

            if (_model.IsEmpty)
            {
                _view.RenderEmpty();
                return;
            }

            _view.Render(_model.BuildViewState());
        }

        public void TierUp()
        {
            _model.TierUp(() =>
            {
                _reShow?.Invoke();
            });
        }
    }
}