using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Synergy", fileName = "Synergy", order = 0)]
    public class SynergyTemplate : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(3, 5)] private string _description;

        [Header("리소스")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _textColor;

        [Header("시너지")]
        [SerializeField] private List<SynergyData> _synergyDatas = new List<SynergyData>();

        #region 프로퍼티
        public int id => _id;
        public string displayName => _displayName;
        public string description => _description;
        public Sprite icon => _icon;
        public Color textColor => _textColor;
        public IReadOnlyList<SynergyData> synergyDatas => _synergyDatas;
        #endregion
    }

    [Serializable]
    public class SynergyData
    {
        [Label("시너지 수")] public int count;
        [Label("시너지 유닛에 적용")] public bool isSynergyUnit;
        [Label("유닛 타입")] public EUnitType unitType;
        [Label("버프")] public BuffTemplate buff;
    }
}