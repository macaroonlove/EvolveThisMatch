using Cysharp.Threading.Tasks;
using FrameWork.NetworkTime;
using System.Collections.Generic;
using System.Threading;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIDepartmentDisposePresenter
    {
        private readonly UIDepartmentDisposeView _view;
        private readonly UIDepartmentDisposeModel _model;

        private readonly Dictionary<int, CancellationTokenSource> _slotTickCts = new Dictionary<int, CancellationTokenSource>();

        public UIDepartmentDisposePresenter(UIDepartmentDisposeView view, UIDepartmentDisposeModel model)
        {
            _view = view;
            _model = model;

            _model.onCreateCraftItem += CreateCraftItem;
        }

        public void Initialize(DepartmentSnapshot snapshot)
        {
            _model.SetSnapshot(snapshot);

            InitializeView().Forget();
        }

        private async UniTask InitializeView()
        {
            // 모든 슬롯 정지
            StopAllTicks();

            var now = await NetworkTimeManager.Instance.GetUtcNow();

            var viewState = _model.BuildViewState(now);
            _view.Render(viewState);

            for (int i = 0; i < viewState.slots.Count; i++)
            {
                EvaluateSlot(i, viewState.slots[i]);
            }
        }

        private void EvaluateSlot(int index, DepartmentDisposeItemViewState slot)
        {
            if (slot.IsEmpty)
                return;

            if (_model.CheckFullStorage(index))
            {
                _view.RenderFullStorage(index);
                return;
            }

            if (_model.CheckLackRequiredItem(index))
            {
                _view.RenderLackRequiredItem(index);
                return;
            }

            StartTick(index);
        }

        private void StartTick(int index)
        {
            StopTick(index);

            var cts = new CancellationTokenSource();
            _slotTickCts[index] = cts;

            TickLoop(index, cts.Token).Forget();
        }

        private async UniTaskVoid TickLoop(int index, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var now = await NetworkTimeManager.Instance.GetUtcNow();
                var slotState = _model.BuildSlotState(index, now);

                _view.RenderSlot(index, slotState);

                if (slotState.IsEmpty || _model.CheckFullStorage(index) || _model.CheckLackRequiredItem(index))
                {
                    StopTick(index);
                    return;
                }

                await UniTask.Delay(1000, cancellationToken: token);
            }
        }

        /// <summary>
        /// 아이템이 생성되었다면
        /// </summary>
        private async void CreateCraftItem()
        {
            var now = await NetworkTimeManager.Instance.GetUtcNow();

            StopAllTicks();

            var viewState = _model.BuildViewState(now);
            _view.Render(viewState);
            _view.CreateCraftItem();

            for (int i = 0; i < viewState.slots.Count; i++)
            {
                EvaluateSlot(i, viewState.slots[i]);
            }
        }

        private void StopTick(int index)
        {
            if (_slotTickCts.TryGetValue(index, out var cts))
            {
                cts.Cancel();
                _slotTickCts.Remove(index);
            }
        }

        public void StopAllTicks()
        {
            foreach (var cts in _slotTickCts.Values)
                cts.Cancel();

            _slotTickCts.Clear();
        }
    }
}