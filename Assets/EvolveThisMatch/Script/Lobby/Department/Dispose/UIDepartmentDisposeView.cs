using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentDisposeView : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            CloseButton,
            ClearDisposeButton,
        }
        #endregion

        private UIDepartmentDisposePresenter _presenter;

        private UIDepartmentDisposeItem[] _slots;

        public event UnityAction<int> onOpenDepartmentDisposeRegistView;
        public event UnityAction<int> onGainItem;
        public event UnityAction<int> onRemoveJob;
        public event UnityAction onClearJob;
        public event UnityAction onCreateCraftItem;

        protected override void Initialize()
        {
            var model = new UIDepartmentDisposeModel();
            _presenter = new UIDepartmentDisposePresenter(this, model);

            _slots = GetComponentsInChildren<UIDepartmentDisposeItem>();
            int slotIndex = 0;
            foreach (var slot in _slots)
            {
                slot.Initailize(slotIndex, 
                    (index) => onOpenDepartmentDisposeRegistView?.Invoke(index), 
                    (index) => onGainItem?.Invoke(index),
                    (index) => onRemoveJob?.Invoke(index));
                slotIndex++;
            }

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.ClearDisposeButton).onClick.AddListener(() => onClearJob?.Invoke());
        }

        public void Initialize(DepartmentSnapshot snapshot)
        {
            _presenter?.Initialize(snapshot);
        }

        public void Render(DepartmentDisposeViewState state)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var slotState = state.slots[i];

                if (!slotState.isUnlocked)
                {
                    _slots[i].Lock(slotState.unlockLevel);
                }
                else
                {
                    _slots[i].Render(slotState);
                }
            }
        }

        public void RenderSlot(int index, DepartmentDisposeItemViewState state)
        {
            _slots[index].Render(state);
        }

        public void RenderFullStorage(int index)
        {
            _slots[index].RenderFullStorage();
        }

        public void RenderLackRequiredItem(int index)
        {
            _slots[index].RenderLackRequiredItem();
        }

        public void CreateCraftItem()
        {
            onCreateCraftItem?.Invoke();
        }

        public void StopTick()
        {
            _presenter?.StopAllTicks();
        }
    }

    public readonly struct DepartmentDisposeViewState
    {
        public readonly IReadOnlyList<DepartmentDisposeItemViewState> slots;

        public DepartmentDisposeViewState(IReadOnlyList<DepartmentDisposeItemViewState> slots)
        {
            this.slots = slots;
        }
    }
}