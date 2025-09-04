using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class DepartmentSaveData
    {
        [Tooltip("각 부서 정보")]
        public List<Department> departments = new List<Department>();

        #region 데이터 모델
        [Serializable]
        public class Department
        {
            public string departmentId;
            public int level;
            public int activeJobCount => activeJobs.Count;

            [SerializeField] private List<CraftingJob> activeJobs = new List<CraftingJob>();

            public Department(string id)
            {
                departmentId = id;
                level = 1;
            }

            public CraftingJob GetActiveJob(int i)
            {
                if (i < 0 || i >= activeJobs.Count) return null;

                return activeJobs[i];
            }

            public bool SetActiveJob(int i, int unitId, int itemId, int maxAmount)
            {
                if (i < 0) return false;

                if (i >= activeJobs.Count)
                {
                    var newJob = new CraftingJob();
                    newJob.unitId = unitId;
                    newJob.craftItemId = itemId;
                    newJob.maxAmount = maxAmount;
                    newJob.startTime = DateTime.UtcNow;
                    activeJobs.Add(newJob);
                }
                else
                {
                    activeJobs[i].unitId = unitId;
                    activeJobs[i].craftItemId = itemId;
                    activeJobs[i].maxAmount = maxAmount;
                    activeJobs[i].startTime = DateTime.UtcNow;
                }

                return true;
            }

            public void RemoveActiveJob(CraftingJob job)
            {
                if (job == null) return;

                activeJobs.Remove(job);
            }
        }

        [Serializable]
        public class CraftingJob
        {
            public ObscuredInt unitId;
            public ObscuredInt craftItemId;
            public ObscuredInt maxAmount;
            [SerializeField] private ObscuredLong startTimeTicks;

            public DateTime startTime
            {
                get => new DateTime(startTimeTicks);
                set => startTimeTicks = value.Ticks;
            }
        }
        #endregion
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/DepartmentSaveData", fileName = "DepartmentSaveData", order = 3)]
    public class DepartmentSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private DepartmentSaveData _data;

        public IReadOnlyList<DepartmentSaveData.Department> departments => _data.departments;

        public override void SetDefaultValues()
        {
            _data = new DepartmentSaveData();

            _data.departments.Add(new DepartmentSaveData.Department("식품부"));
            _data.departments.Add(new DepartmentSaveData.Department("가공부"));
            _data.departments.Add(new DepartmentSaveData.Department("건설부"));

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<DepartmentSaveData>(json);

            if (_data != null)
            {
                isLoaded = departments.Count > 0;
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }
    }
}