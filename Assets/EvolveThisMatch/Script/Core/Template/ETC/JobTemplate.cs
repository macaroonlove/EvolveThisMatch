using UnityEngine;

namespace EvolveThisMatch.Core
{
    public enum EJob
    {
        Melee,
        Wizard,
        Summoner,
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