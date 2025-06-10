using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class SkillTemplate : ScriptableObject, IDataWindowEntry
    {
        [HideInInspector, SerializeField] protected Sprite _sprite;

        [HideInInspector, SerializeField] protected int _id;
        [HideInInspector, SerializeField] protected string _displayName;
        [HideInInspector, SerializeField] protected string _description;

        #region 프로퍼티
        public Sprite sprite => _sprite;
        public int id => _id;
        public string displayName => _displayName;
        public string description => _description;
        #endregion

        #region 값 변경 메서드
        internal void SetId(int id) => _id = id;
        public void SetDisplayName(string name) => _displayName = name;
        internal void SetDescription(string desc) => _description = desc;
        #endregion
    }
}