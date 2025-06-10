using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class FormationSlot
    {
        public int id;
        // TODO: 아이템 등 확장하기
    }

    [Serializable]
    public class FormationSaveData
    {
        public string displayName;

        [Tooltip("포메이션")]
        public List<FormationSlot> formation = new List<FormationSlot>();
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/FormationSaveData", fileName = "FormationSaveData", order = 0)]
    public class FormationSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private FormationSaveData _data;

        public bool isLoaded { get; private set; }

        public string displayName { get => _data.displayName; set => _data.displayName = value; }

        public IReadOnlyList<FormationSlot> formation => _data.formation;

        public override void SetDefaultValues()
        {
            _data = new FormationSaveData();

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<FormationSaveData>(json);

            if (_data != null)
            {
                isLoaded = _data.formation.Count > 0;
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

        public void UpdateFormation(List<FormationSlot> formation)
        {
            _data.formation = formation;
        }
    }
}