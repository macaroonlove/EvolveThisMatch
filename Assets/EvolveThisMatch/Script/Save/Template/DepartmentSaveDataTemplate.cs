using FrameWork.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
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
                if (i <= 0 || i > activeJobs.Count) return null;

                return activeJobs[i];
            }
        }

        [Serializable]
        public class CraftingJob
        {
            public int chargeUnitId;
            public int craftItemId;
            public int maxAmount;
            public DateTime startTime;
        }
        #endregion
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/DepartmentSaveData", fileName = "DepartmentSaveData", order = 2)]
    public class DepartmentSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private DepartmentSaveData _data;

        public bool isLoaded { get; private set; }

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

                _departmentDic = departments.ToDictionary(d => d.departmentId);
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

        #region 제작
        private Dictionary<string, DepartmentSaveData.Department> _departmentDic;

        public void SetDepartment(DepartmentSaveData.Department department)
        {
            _departmentDic[department.departmentId] = department;
        }
        #endregion
    }
}