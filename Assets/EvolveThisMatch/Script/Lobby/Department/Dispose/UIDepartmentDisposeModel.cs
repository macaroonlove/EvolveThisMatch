using EvolveThisMatch.Save;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIDepartmentDisposeModel
    {
        private DepartmentSnapshot _snapshot;
        private Dictionary<int, int> _lastCraftCount = new Dictionary<int, int>();

        public event UnityAction onCreateCraftItem;

        public void SetSnapshot(DepartmentSnapshot snapshot)
        {
            _snapshot = snapshot;

            _lastCraftCount.Clear();
            for (int i = 0; i < snapshot.slots.Count; i++)
            {
                _lastCraftCount[i] = snapshot.slots[i].craftCount;
            }
        }

        public DepartmentDisposeViewState BuildViewState(DateTime nowUtc)
        {
            var list = new List<DepartmentDisposeItemViewState>(_snapshot.slots.Count);

            for (int i = 0; i < _snapshot.slots.Count; i++)
            {
                list.Add(BuildSlotState(i, nowUtc));
            }

            return new DepartmentDisposeViewState(list);
        }

        public DepartmentDisposeItemViewState BuildSlotState(int index, DateTime nowUtc)
        {
            var slot = _snapshot.slots[index];

            // 비활성화된 슬롯이라면 잠금 상태만 반환
            if (!slot.isActive)
            {
                return new DepartmentDisposeItemViewState(index, slot.slotContext.isUnlock, slot.slotContext.unlockLevel, null, 0, 0, 0, 0, 0, 0, TimeSpan.Zero);
            }

            // 현재 사이클 시작 시점
            float cycleElapsed = slot.slotContext.interval - slot.remainTime;

            // 경과 시간
            var elapsed = nowUtc - _snapshot.snapshotTimeUtc;
            float elapsedSec = (float)elapsed.TotalSeconds;

            // 현재 사이클 기준 경과 시간
            float currentCycleElapsed = cycleElapsed + elapsedSec;

            // 추가로 생산한 수
            int additionalCount = Mathf.FloorToInt(currentCycleElapsed / slot.slotContext.interval);
            int totalCount = slot.craftCount + additionalCount;

            // 변화 감지
            if (_lastCraftCount.TryGetValue(index, out var prevCount))
            {
                if (totalCount != prevCount)
                {
                    _lastCraftCount[index] = totalCount;
                    onCreateCraftItem?.Invoke();
                }
            }

            // 남은 생산 개수
            int remainingCount = slot.slotContext.job.maxAmount - totalCount;

            // 진행도
            float progress = Mathf.Clamp01((currentCycleElapsed % slot.slotContext.interval) / slot.slotContext.interval);
            var remainTime = TimeSpan.FromSeconds(slot.slotContext.interval * (1f - progress));
            
            return new DepartmentDisposeItemViewState(index, slot.slotContext.isUnlock, slot.slotContext.unlockLevel, slot.slotContext.item, slot.slotContext.job.unitId, slot.slotContext.speed, slot.slotContext.item.Weight, totalCount, remainingCount, progress, remainTime);
        }

        /// <summary>
        /// 보관량이 가득찼는지 검사
        /// </summary>
        public bool CheckFullStorage(int index)
        {
            // 보관함 무게 <= 스냅샷 시점 총 무게 + (아이템의 무게 * (최종 생산 개수 - 스냅샷 생산 개수 + 1))
            return _snapshot.levelData.StorageWeight <= _snapshot.snapshotTotalWeight + (_snapshot.slots[index].slotContext.item.Weight * (_lastCraftCount[index] - _snapshot.slots[index].craftCount + 1));
        }

        /// <summary>
        /// 재료가 부족한지 검사
        /// </summary>
        public bool CheckLackRequiredItem(int index)
        {
            foreach (var required in _snapshot.slots[index].slotContext.item.RequiredItems)
            {
                var requiredItem = SaveManager.Instance.profileData.GetVariable(required.Variable);
                if (requiredItem.Value - _snapshot.slots[index].craftCount < required.Amount)
                {
                    // 재료가 충분하지 않다면
                    return true;
                }
            }

            return false;
        }
    }
}