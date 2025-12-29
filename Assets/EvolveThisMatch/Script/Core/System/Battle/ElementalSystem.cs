using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ElementalSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private List<ElementalTemplate> _elementalTemplates;

        private Dictionary<ElementalTemplate, ElementalData> _elementalDatas = new Dictionary<ElementalTemplate, ElementalData>();

        public void Initialize()
        {
            _elementalDatas.Clear();

            foreach (var template in _elementalTemplates)
            {
                _elementalDatas.Add(template, new ElementalData());
            }
        }

        public void Deinitialize()
        {
            _elementalDatas.Clear();
        }

        public int RequestIncreaseElemental(ElementalTemplate template)
        {
            var data = _elementalDatas[template];

            int token = data.PrepareElementalIncrease();
            return data.ApplyElementalIncrease(token);
        }

        public int GetLevel(ElementalTemplate template)
        {
            var data = _elementalDatas[template];

            return data.level;
        }

        public int GetNeedCrystal(ElementalTemplate template)
        {
            var data = _elementalDatas[template];

            return data.level + 1;
        }
    }
}