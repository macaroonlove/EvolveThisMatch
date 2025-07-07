using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public enum ESkillType
    {
        Divine,
        Dark,
        Fire,
        Water,
        Earth,
        Wind,
        Thunder,
    }

    [CreateAssetMenu(menuName = "Templates/Etc/SkillType", fileName = "SkillType", order = 0)]
    public class SkillTypeTemplate : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private ESkillType _type;
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _textColor;

        [SerializeField] private TypeEngraveData[] _engraveDatas = new TypeEngraveData[5];

        #region 프로퍼티
        public string displayName => _displayName;
        public ESkillType type => _type;
        public Color backgroundColor => _backgroundColor;
        public Color textColor => _textColor;

        public int engraveLevel { get; private set; }
        #endregion

        public void Initialize()
        {
            engraveLevel = 0;
        }

        public TypeEngraveData GetEngraveData()
        {
            return _engraveDatas[engraveLevel];
        }

        public bool UpgradeEngraveLevel()
        {
            if (engraveLevel >= _engraveDatas.Length - 1) return false;

            engraveLevel++;
            return true;
        }
    }

    [Serializable]
    public class TypeEngraveData
    {
        public int needCrystal;
    }
}