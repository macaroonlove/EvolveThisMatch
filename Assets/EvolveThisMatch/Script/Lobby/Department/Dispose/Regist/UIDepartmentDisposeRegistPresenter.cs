using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIDepartmentDisposeRegistPresenter
    {
        private readonly UIDepartmentDisposeRegistView _view;
        private readonly UIDepartmentDisposeRegistModel _model;

        public UIDepartmentDisposeRegistPresenter(UIDepartmentDisposeRegistView view, UIDepartmentDisposeRegistModel model)
        {
            _view = view;
            _model = model;
        }

        public void Bind(DepartmentData titleData, DepartmentLocalSaveData localData) => _model.Bind(titleData, localData);

        public void Show(int slotIndex)
        {
            _model.SelectSlot(slotIndex);

            _view.Render(_model.BuildViewState());
        }

        public void SelectAgent(int agentId)
        {
            _model.SelectAgent(agentId);

            _view.RenderCount(_model.BuildViewState());
        }

        public void SelectCraftItem(int itemIndex)
        {
            _model.SelectCraftItem(itemIndex);

            _view.RenderCount(_model.BuildViewState());
        }

        public void ChangeCounter(int value)
        {
            _model.ChangeCounter(value);

            _view.RenderCount(_model.BuildViewState());
        }

        public void Confilm(UnityAction<int, int, int, int> onRegistJob)
        {
            _model.Confilm(onRegistJob);
            _view.Hide(true);
        }
    }
}