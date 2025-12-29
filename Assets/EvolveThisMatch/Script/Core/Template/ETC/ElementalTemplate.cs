using UnityEngine;

namespace EvolveThisMatch.Core
{
    public enum EElementalType
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
    public class ElementalTemplate : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private EElementalType _type;
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _textColor;

        #region 프로퍼티
        public string displayName => _displayName;
        public EElementalType type => _type;
        public Color backgroundColor => _backgroundColor;
        public Color textColor => _textColor;
        #endregion
    }
}