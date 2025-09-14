using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [CreateAssetMenu(menuName = "Templates/Lobby/Department", fileName = "Department_", order = 1)]
    public class DepartmentTemplate : ScriptableObject
    {
        [SerializeField] private string _departmentName;
        [SerializeField, TextArea] private string _departmentDescription;
        [SerializeField] private Sprite _departmentBackground;
        [SerializeField] private Vector2[] _unitPos;

        [Space(10)]
        [SerializeField] private List<CraftItemData> _craftItems = new List<CraftItemData>();

        [SerializeField]
        private List<DepartmentLevelData> _levelDatas = new List<DepartmentLevelData>();        

        #region 프로퍼티
        internal string departmentName => _departmentName;
        internal string departmentDescription => _departmentDescription;
        internal Sprite departmentBackground => _departmentBackground;
        internal Vector2[] unitPos => _unitPos;

        internal IReadOnlyList<CraftItemData> craftItems => _craftItems;
        #endregion

        internal DepartmentLevelData GetLevelData(int level)
        {
            if (level <= 0 || level > _levelDatas.Count) return null;

            return _levelDatas[level - 1];
        }

        internal int GetUnLockMaxUnitLevel(int index)
        {
            for (int i = 0; i < _levelDatas.Count; i++)
            {
                if (index + 1 == _levelDatas[i].maxUnits)
                {
                    return i + 1;
                }
            }

            return 0;
        }
    }

    [Serializable]
    public class CraftItemData
    {
        [SerializeField, Label("제작품")] private ObscuredIntVariable _variable;
        [SerializeField, Label("제작 시간(초)")] private int _craftTime;
        [SerializeField, Label("무게(g)")] private int _weight;
        [SerializeField] private RequiredItem[] _requiredItems;

        internal ObscuredIntVariable variable => _variable;
        internal int craftTime => _craftTime;
        internal int weight => _weight;
        internal IReadOnlyList<RequiredItem> requiredItems => _requiredItems;

        [Serializable]
        public class RequiredItem
        {
            [SerializeField, Label("재료")] private ObscuredIntVariable _item;
            [SerializeField, Label("개수")] private int _amount;

            internal ObscuredIntVariable item => _item;
            internal int amount => _amount;
        }
    }

    [Serializable]
    public class DepartmentLevelData
    {
        [Tooltip("최대 인원 수")]
        public int maxUnits;

        [Tooltip("보관 가능한 총 무게")]
        public int storageWeight;

        [Tooltip("생산 속도 배율")]
        public float speed;
    }
}