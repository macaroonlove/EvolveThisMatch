using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class RarityTemplate : ScriptableObject
    {
        [SerializeField] private int _order;
        [SerializeField] private string _displayName;

        #region 프로퍼티
        public int order => _order;
        public string displayName => _displayName;
        #endregion
    }
}