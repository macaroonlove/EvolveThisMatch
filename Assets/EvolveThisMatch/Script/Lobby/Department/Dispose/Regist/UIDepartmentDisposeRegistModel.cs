using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIDepartmentDisposeRegistModel
    {
        private DepartmentData _titleData;
        private DepartmentLocalSaveData _localData;

        private int _selectedSlotIndex;
        private int _selectedAgentId;
        private int _selectedItemIndex;
        private int _craftCount;

        public void Bind(DepartmentData titleData, DepartmentLocalSaveData localData)
        {
            _titleData = titleData;
            _localData = localData;
        }

        public void SelectSlot(int slotIndex)
        {
            _selectedSlotIndex = slotIndex;

            // 이미 생산 중이라면 이전 최대 개수만큼
            var job = _localData.GetJob(slotIndex);
            if (job != null) _craftCount = job.maxAmount;

            _craftCount = Mathf.Max(1, _craftCount);
        }

        public void SelectAgent(int agentId) => _selectedAgentId = agentId;
        public void SelectCraftItem(int itemIndex) => _selectedItemIndex = itemIndex;
        public void ChangeCounter(int value) => _craftCount = value;

        public DepartmentDisposeRegistViewState BuildViewState()
        {
            if (_titleData == null) return new DepartmentDisposeRegistViewState();

            var requiredItems = _titleData.CraftItems[_selectedItemIndex].RequiredItems;

            int maxCraftableCount = 999;

            foreach (var required in requiredItems)
            {
                var variable = SaveManager.Instance.profileData.GetVariable(required.Variable);
                int ownedCount = variable.Value;
                int craftableCount = ownedCount / required.Amount;

                maxCraftableCount = Mathf.Min(maxCraftableCount, craftableCount);
            }

            List<int> deployList = new List<int>();
            bool isDeploy = false;
            foreach (var job in _localData.jobs)
            {
                deployList.Add(job.unitId);

                if (job.unitId == _selectedAgentId) isDeploy = true;
            }

            return new DepartmentDisposeRegistViewState(_titleData, _craftCount, maxCraftableCount, isDeploy, deployList);
        }

        public void Confilm(UnityAction<int, int, int, int> onRegistJob)
        {
            onRegistJob?.Invoke(_selectedSlotIndex, _selectedAgentId, _selectedItemIndex, _craftCount);
        }
    }
}