using FrameWork.PlayFabExtensions;
using System;
using System.Collections.Generic;

namespace EvolveThisMatch.Lobby
{
    public sealed class DepartmentSnapshot
    {
        public readonly string departmentId;
        public readonly DepartmentLevelData levelData;
        public readonly int snapshotTotalWeight;
        public readonly DateTime snapshotTimeUtc;
        public readonly IReadOnlyList<DepartmentSlotSnapshot> slots;

        public DepartmentSnapshot(string departmentId, DepartmentLevelData levelData, int snapshotTotalWeight, DateTime snapshotTimeUtc, IReadOnlyList<DepartmentSlotSnapshot> slots)
        {
            this.departmentId = departmentId;
            this.levelData = levelData;
            this.snapshotTotalWeight = snapshotTotalWeight;
            this.snapshotTimeUtc = snapshotTimeUtc;
            this.slots = slots;
        }
    }

    public sealed class DepartmentSlotSnapshot
    {
        /// <summary>
        /// 슬롯 정적 데이터
        /// </summary>
        public SlotContext slotContext { get; }

        /// <summary>
        /// 스냅샷 시점 생산량
        /// </summary>
        public int craftCount { get; }

        /// <summary>
        /// 스냅샷 시점 슬롯의 남은 생산 시간 (초)
        /// </summary>
        public float remainTime { get; }
        
        /// <summary>
        /// 슬롯 활성화 여부
        /// </summary>
        public bool isActive { get; }

        public DepartmentSlotSnapshot(SlotContext slotContext, int craftCount, float remainTime, bool isActive)
        {
            this.slotContext = slotContext;
            this.craftCount = craftCount;
            this.remainTime = remainTime;
            this.isActive = isActive;
        }
    }
}