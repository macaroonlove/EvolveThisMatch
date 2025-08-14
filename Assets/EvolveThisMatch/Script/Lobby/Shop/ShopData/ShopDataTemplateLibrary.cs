using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [CreateAssetMenu(menuName = "Templates/Lobby/ShopDataLibrary", fileName = "ShopDataLibrary", order = 0)]
    public class ShopDataTemplateLibrary : ScriptableObject
    {
        [SerializeField] private List<ShopDataTemplate> _shopDataTemplates = new List<ShopDataTemplate>();

        public IReadOnlyList<ShopDataTemplate> shopDataTemplates => _shopDataTemplates;
    }
}