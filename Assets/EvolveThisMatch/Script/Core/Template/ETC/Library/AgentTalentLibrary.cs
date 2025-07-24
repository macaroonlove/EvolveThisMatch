using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/AgentTalent", fileName = "AgentTalentLibrary", order = 0)]
    public class AgentTalentLibrary : ScriptableObject
    {
        [SerializeField] private List<AgentTalentData> _talentData = new List<AgentTalentData>();

        [Header("재능 등급별 확률정보")]
        [SerializeField, Range(0, 100.0f)] private int _mythRarity;
        [SerializeField, Range(0, 100.0f)] private int _legendRarity;
        [SerializeField, Range(0, 100.0f)] private int _epicRarity;
        [SerializeField, Range(0, 100.0f)] private int _rareRarity;
        [SerializeField, Range(0, 100.0f)] private int _commonRarity;

        private float[] _rarityProbabilities;

        internal void Initialize()
        {
            _rarityProbabilities = new float[5];
            _rarityProbabilities[0] = _mythRarity;
            _rarityProbabilities[1] = _legendRarity;
            _rarityProbabilities[2] = _epicRarity;
            _rarityProbabilities[3] = _rareRarity;
            _rarityProbabilities[4] = _commonRarity;
        }

        internal List<AgentTalentData> GetAllTalentEffect()
        {
            return _talentData;
        }

        internal AgentTalentData GetTalentEffect(int id)
        {
            return _talentData[id];
        }

        internal AgentTalentData GetRandomTalentEffect()
        {
            return _talentData[Random.Range(0, _talentData.Count)];
        }

        internal AgentRarityTemplate GetRandomRarity()
        {
            var raritys = GameDataManager.Instance.agentRarityTemplates;

            float rand = Random.Range(0, 100.0f);
            float cumulativeProbability = 0;

            for (int i = 0; i < _rarityProbabilities.Length; i++)
            {
                cumulativeProbability += _rarityProbabilities[i];
                if (rand <= cumulativeProbability)
                {
                    return raritys[i];
                }
            }

            return raritys[_rarityProbabilities.Length - 1];
        }
    }

    [Serializable]
    public class AgentTalentData
    {
        public int id;
        public Effect effect;
        public int minValue;
        public int maxValue;

        public int rareLimit;
        public int epicLimit;
        public int legendLimit;
        public int mythLimit;

        public AgentRarityTemplate GetRarity(int value)
        {
            var raritys = GameDataManager.Instance.agentRarityTemplates;

            if (value >= mythLimit) return raritys[0];
            else if (value >= legendLimit) return raritys[1];
            else if (value >= epicLimit) return raritys[2];
            else if (value >= rareLimit) return raritys[3];

            return raritys[4];
        }
    }
}