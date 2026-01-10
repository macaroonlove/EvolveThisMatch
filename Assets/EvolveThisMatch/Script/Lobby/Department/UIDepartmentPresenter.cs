using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIDepartmentPresenter
    {
        private readonly UIDepartmentCanvas _view;
        private readonly UIDepartmentModel _model;

        public UIDepartmentPresenter(UIDepartmentCanvas view, UIDepartmentModel model)
        {
            _view = view;
            _model = model;
        }

        public async UniTask ChangeDepartment(DepartmentSaveData.Department userData, DepartmentData titleData, DepartmentLocalSaveData localData)
        {
            _model.Bind(userData, titleData, localData);

            _view.DeselectItems();
            await Refresh();
        }

        public async UniTask Refresh()
        {
            var viewState = await _model.BuildViewState();

            _view.Render(viewState);
        }

        public async UniTask UpdateInfoViewState()
        {
            var viewState = await _model.UpdateInfoViewState();

            _view.UpdateInfoRender(viewState);
        }

        public async UniTask GainItem(int index)
        {
            await _model.GainItem(index);

            await Refresh();
        }

        public async UniTask BundleGainItem()
        {
            await _model.BundleGainItem();

            await Refresh();
        }

        public async UniTask RegistJob(int slotIndex, int agentId, int itemId, int count)
        {
            // æ∆¿Ã≈€ »πµÊ
            await _model.GainItem(slotIndex);

            _model.RegistJob(slotIndex, agentId, itemId, count);

            await Refresh();
        }

        public async UniTask RemoveJob(int index)
        {
            // æ∆¿Ã≈€ »πµÊ
            await _model.GainItem(index);

            _model.RemoveJob(index);

            await Refresh();
        }

        public async UniTask ClearJob()
        {
            // ∏µÁ æ∆¿Ã≈€ »πµÊ
            await _model.BundleGainItem();

            _model.ClearJob();

            await Refresh();
        }
    }
}