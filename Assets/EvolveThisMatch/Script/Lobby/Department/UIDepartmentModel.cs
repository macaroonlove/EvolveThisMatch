using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.NetworkTime;
using FrameWork.PlayFabExtensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIDepartmentModel
    {
        private DepartmentSaveData.Department _userData;
        private DepartmentData _titleData;
        private DepartmentLocalSaveData _localData;

        private DepartmentSnapshot _snapshot;

        public void Bind(DepartmentSaveData.Department userData, DepartmentData titleData, DepartmentLocalSaveData localData)
        {
            _userData = userData;
            _titleData = titleData;
            _localData = localData;
        }

        #region View State 생성
        public async UniTask<DepartmentViewState> BuildViewState()
        {
            _snapshot = await BuildSnapshot();
            var info = BuildInfoViewState();
            var background = await AddressableAssetManager.Instance.GetSpriteAsync(_titleData.Background);

            return new DepartmentViewState(info, _snapshot, background, _titleData, _localData);
        }

        private DepartmentInfoViewState BuildInfoViewState()
        {
            var levelData = GetDepartmentLevelData();

            return new DepartmentInfoViewState
            {
                title = _titleData.DepartmentName,
                description = _titleData.Description,
                level = _userData.level,
                activeCount = _localData.activeWorkbenchCount,
                maxUnit = levelData.MaxUnits,
                totalWeight = _snapshot.snapshotTotalWeight,
                maxWeight = levelData.StorageWeight,
                speed = levelData.Speed
            };
        }

        public async UniTask<DepartmentInfoViewState> UpdateInfoViewState()
        {
            int totalWeight = 0;
            var utcNow = await NetworkTimeManager.Instance.GetUtcNow();
            var levelData = GetDepartmentLevelData();

            foreach (var slot in _snapshot.slots)
            {
                // 현재 사이클 시작 시점
                float cycleElapsed = slot.slotContext.interval - slot.remainTime;

                // 경과 시간
                var elapsed = utcNow - _snapshot.snapshotTimeUtc;
                float elapsedSec = (float)elapsed.TotalSeconds;

                // 현재 사이클 기준 경과 시간
                float currentCycleElapsed = cycleElapsed + elapsedSec;

                // 추가로 생산한 수
                int additionalCount = Mathf.FloorToInt(currentCycleElapsed / slot.slotContext.interval);
                int totalCount = slot.craftCount + additionalCount;

                // 무게에 합산
                totalWeight += slot.slotContext.item.Weight * totalCount;
            }

            return new DepartmentInfoViewState
            {
                title = _titleData.DepartmentName,
                description = _titleData.Description,
                level = _userData.level,
                activeCount = _localData.activeWorkbenchCount,
                maxUnit = levelData.MaxUnits,
                totalWeight = totalWeight,
                maxWeight = levelData.StorageWeight,
                speed = levelData.Speed
            };
        }
        #endregion  

        #region 부서 활성화 시, 상태 스냅샷
        private async UniTask<DepartmentSnapshot> BuildSnapshot()
        {
            var utcNow = await NetworkTimeManager.Instance.GetUtcNow();
            var levelData = GetDepartmentLevelData();

            var contexts = BuildSlotContexts(levelData);
            var craftResults = CalculateCraftResults(utcNow, levelData, contexts);

            var slots = new List<DepartmentSlotSnapshot>(_localData.workbenchCount);
            int totalWeight = 0;

            foreach (var context in contexts)
            {
                var result = craftResults.GetValueOrDefault(context.slotIndex);

                // 스냅샷 시점 슬롯의 남은 생산 시간 (초)
                TimeSpan elapsed = utcNow - context.startTime;
                float elapsedSeconds = (float)elapsed.TotalSeconds;
                float remainTime = context.interval - (elapsedSeconds % context.interval);

                // 작업의 활성화 여부 받아오기
                bool isActive = false;
                var job = _localData.GetJob(context.slotIndex);
                if (job != null) isActive = job.isActive;

                // 총 무게 계산
                totalWeight += result.craftCount * context.item.Weight;

                slots.Add(new DepartmentSlotSnapshot(context, result.craftCount, remainTime, isActive));
            }

            return new DepartmentSnapshot(_titleData.DepartmentName, levelData, totalWeight, utcNow, slots);
        }

        private List<SlotContext> BuildSlotContexts(DepartmentLevelData levelData)
        {
            var contexts = new List<SlotContext>(_localData.workbenchCount);

            for (int i = 0; i < _localData.workbenchCount; i++)
            {
                var job = _localData.GetJob(i);
                if (job == null) continue;

                var item = _titleData.CraftItems[job.craftItemId];
                float agentLevel = SaveManager.Instance.agentData.GetAgent(job.unitId).level;

                float speed = agentLevel * 0.01f + levelData.Speed;
                float interval = item.CraftTime / speed;

                bool isUnlock = i < levelData.MaxUnits;
                int unlockLevel = _titleData.GetUnLockMaxUnitLevel(i);

                contexts.Add(new SlotContext(i, job, item, speed, interval, isUnlock, unlockLevel));
            }

            return contexts;
        }

        private Dictionary<int, CraftResult> CalculateCraftResults(DateTime now, DepartmentLevelData levelData, List<SlotContext> contexts)
        {
            // 모든 작업을 시간 순으로 정렬하기 위해 리스트 생성
            var allJobs = new List<(int slotIndex, float finishTime, int weight)>();
            var results = new Dictionary<int, CraftResult>();

            foreach (var context in contexts)
            {
                // 작업 경과시간을 계산하기
                TimeSpan elapsed = now - context.startTime;
                float elapsedSeconds = (float)elapsed.TotalSeconds;

                // 시간을 기준으로 생산한 계수 계산하기
                int craftCount = Mathf.Min(context.job.maxAmount, Mathf.FloorToInt(elapsedSeconds / context.interval));

                // 재료를 기준으로 생산한 계수 계산하기
                foreach (var required in context.item.RequiredItems)
                {
                    var variable = SaveManager.Instance.profileData.GetVariable(required.Variable);
                    int craftableCount = variable.Value / required.Amount;

                    craftCount = Mathf.Min(craftCount, craftableCount);
                }

                // 모든 작업의 예상 종료 시간을 저장하기
                for (int j = 0; j < craftCount; j++)
                {
                    float finishTime = (float)(context.startTime.AddSeconds(context.interval * (j + 1)) - DateTime.UnixEpoch).TotalSeconds;

                    allJobs.Add((context.slotIndex, finishTime, context.item.Weight));
                }

                // 해당 작업대를 딕셔너리에 추가
                results[context.slotIndex] = new CraftResult(0, 0);
            }

            // 모든 작업을 작업 종료 시간 순으로 정렬
            allJobs.Sort((a, b) => a.finishTime.CompareTo(b.finishTime));

            // 최대 무게 까지만 아이템이 생성되도록 결과 저장
            float usedWeight = 0f;

            foreach (var job in allJobs)
            {
                if (usedWeight + job.weight > levelData.StorageWeight) continue;

                var result = results[job.slotIndex];
                results[job.slotIndex] = result.Increment(job.weight);

                usedWeight += job.weight;
            }

            return results;
        }

        private readonly struct CraftResult
        {
            public readonly int craftCount;
            public readonly int totalWeight;

            public CraftResult(int craftCount, int totalWeight)
            {
                this.craftCount = craftCount;
                this.totalWeight = totalWeight;
            }

            public CraftResult Increment(int weight)
            {
                return new CraftResult(craftCount + 1, totalWeight + weight);
            }
        }

        private DepartmentLevelData GetDepartmentLevelData()
        {
            return _titleData.GetLevelData(_userData.level);
        }
        #endregion

        public async UniTask GainItem(int index)
        {
            var job = _localData.GetJob(index);
            if (job == null || job.isActive == false) return;

            _snapshot = await BuildSnapshot();

            var slot = _snapshot.slots[index];
            if (slot.craftCount > 0)
            {
                await SaveManager.Instance.departmentData.GainCraftItem(_localData.departmentId, index, slot.craftCount, _snapshot.snapshotTimeUtc, slot.remainTime);
            }
        }

        public async UniTask BundleGainItem()
        {
            _snapshot = await BuildSnapshot();

            List<int> craftCounts = new List<int>();
            List<float> remainTimes = new List<float>();

            bool isWorking = false;
            foreach (var slot in _snapshot.slots)
            {
                if (slot.isActive) isWorking = true;

                craftCounts.Add(slot.craftCount);
                remainTimes.Add(slot.remainTime);
            }

            if (!isWorking) return;

            await SaveManager.Instance.departmentData.BundleGainCraftItem(_localData.departmentId, craftCounts, _snapshot.snapshotTimeUtc, remainTimes);
        }

        public void RegistJob(int slotIndex, int agentId, int itemId, int count)
        {
            // 작업대 등록
            _localData.RegistJob(slotIndex, agentId, itemId, count);

            // 저장
            SaveManager.Instance.departmentData.SaveDepartmentLocalData();
        }

        public void RemoveJob(int index)
        {
            // 작업대 비우기
            _localData.RemoveJob(index);

            // 저장
            SaveManager.Instance.departmentData.SaveDepartmentLocalData();
        }

        public void ClearJob()
        {
            // 모든 작업대 비우기
            _localData.ClearJob();

            // 저장
            SaveManager.Instance.departmentData.SaveDepartmentLocalData();
        }
    }

    public readonly struct SlotContext
    {
        public readonly int slotIndex;
        public readonly DepartmentLocalSaveData.CraftingJob job;
        public readonly DepartmentCraftData item;
        public readonly float speed;
        public readonly float interval;
        public readonly bool isUnlock;
        public readonly int unlockLevel;
        public readonly DateTime startTime;

        public SlotContext(int slotIndex, DepartmentLocalSaveData.CraftingJob job, DepartmentCraftData item, float speed, float interval, bool isUnlock, int unlockLevel)
        {
            this.slotIndex = slotIndex;
            this.job = job;
            this.item = item;
            this.speed = speed;
            this.interval = interval;
            this.isUnlock = isUnlock;
            this.unlockLevel = unlockLevel;
            this.startTime = job.startTime;
        }
    }
}