using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Rarity/PassiveItemRarity", fileName = "PassiveItemRarity", order = 0)]
    public class PassiveItemRarityTemplate : RarityTemplate
    {
        [SerializeField] private EPassiveItemRarity _rarity;
        [SerializeField] private Sprite _sprite;

        #region 프로퍼티
        public EPassiveItemRarity rarity => _rarity;
        public Sprite sprite => _sprite;
        #endregion
    }
}