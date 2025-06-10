using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Rarity/SkinRarity", fileName = "SkinRarity", order = 0)]
    public class SkinRarityTemplate : RarityTemplate
    {
        [SerializeField] private ESkinRarity _rarity;
        [SerializeField] private Sprite _sprite;

        #region 프로퍼티
        public ESkinRarity rarity => _rarity;
        public Sprite sprite => _sprite;
        #endregion
    }
}