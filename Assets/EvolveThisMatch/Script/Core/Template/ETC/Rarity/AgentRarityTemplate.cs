using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Rarity/AgentRarity", fileName = "AgentRarity", order = 0)]
    public class AgentRarityTemplate : RarityTemplate
    {
        [SerializeField] private EAgentRarity _rarity;
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _textColor;

        [Header("업그레이드")]
        [SerializeField, Range(0, 100)] private float _successProbability;

        #region 프로퍼티
        public EAgentRarity rarity => _rarity;
        public Color backgroundColor => _backgroundColor;
        public Color textColor => _textColor;

        public float successProbability => _successProbability;
        #endregion
    }
}