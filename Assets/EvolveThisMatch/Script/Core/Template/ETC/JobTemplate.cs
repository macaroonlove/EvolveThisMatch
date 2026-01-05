using UnityEngine;

namespace EvolveThisMatch.Core
{
    [System.Flags]
    public enum EJob
    {
        None = 0,
        Melee = 1 << 0,
        Wizard = 1 << 1,
        Summoner = 1 << 2,
    }

    [CreateAssetMenu(menuName = "Templates/Etc/Job", fileName = "Job", order = 1)]
    public class JobTemplate : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private EJob _job;
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _textColor;

        #region 프로퍼티
        public string displayName => _displayName;
        public EJob job => _job;
        public Color backgroundColor => _backgroundColor;
        public Color textColor => _textColor;
        #endregion
    }
}