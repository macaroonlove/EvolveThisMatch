using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Rarity/EnemyRarity", fileName = "EnemyRarity", order = 0)]
    public class EnemyRarityTemplate : RarityTemplate
    {
        [SerializeField] private EEnemyRarity _rarity;
        [SerializeField] private Sprite _sprite;

        #region 프로퍼티
        public EEnemyRarity rarity => _rarity;
        public Sprite sprite => _sprite;
        #endregion
    }
}