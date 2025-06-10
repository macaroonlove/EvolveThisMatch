using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Rarity/AgentRarity", fileName = "AgentRarity", order = 0)]
    public class AgentRarityTemplate : RarityTemplate
    {
        [SerializeField] private EAgentRarity _rarity;
        [SerializeField] private Sprite _sprite;

        #region 프로퍼티
        public EAgentRarity rarity => _rarity;
        public Sprite sprite => _sprite;
        #endregion
    }
}