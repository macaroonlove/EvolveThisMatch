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

        #region 프로퍼티
        public string displayName => _displayName;
        public ESkillType type => _type;
        public Color backgroundColor => _backgroundColor;
        public Color textColor => _textColor;
        #endregion
    }
}