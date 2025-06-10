using UnityEngine;

namespace EvolveThisMatch.Core
{
    public enum EJob
    {
        Melee,
        Tanker,
        Ranger,
        Wizard,
        Healer,
        Supporter,
    }

    [CreateAssetMenu(menuName = "Templates/Etc/Job", fileName = "Job", order = 1)]
    public class JobTemplate : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private EJob _job;
        [SerializeField] private Sprite _sprite;

        #region 프로퍼티
        public string displayName => _displayName;
        public EJob job => _job;
        public Sprite sprite => _sprite;
        #endregion
    }
}