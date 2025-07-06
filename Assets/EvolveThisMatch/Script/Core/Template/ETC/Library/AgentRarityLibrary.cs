using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/AgentRarity", fileName = "AgentRarityLibrary", order = 0)]
    public class AgentRarityLibrary : ScriptableObject
    {
        [SerializeField] private List<AgentRarityTemplate> _agentRarityTemplates = new List<AgentRarityTemplate>();
        [SerializeField] private List<AgentRarityProbabilityList> _probabilityList = new List<AgentRarityProbabilityList>();

        public int probabilityLevel { get; private set; }

        public void Initialize()
        {
            probabilityLevel = 0;
        }

        public AgentRarityTemplate GetRandomAgentRarityTemplate()
        {
            var prob = _probabilityList[probabilityLevel];

            float[] probabilities = new float[]
            {
                prob.myth,
                prob.legend,
                prob.epic,
                prob.rare,
                prob.common
            };

            float rand = Random.Range(0f, 100f);
            float cumulative = 0f;
            int index = 0;

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulative += probabilities[i];
                if (rand < cumulative)
                {
                    index = i;
                    break;
                }
            }
            
            return _agentRarityTemplates[index];
        }

        public AgentRarityTemplate GetUpgradeAgentRarityTemplate(AgentRarityTemplate currentAgentRarity)
        {
            float rand = Random.Range(0, 100);
            if (currentAgentRarity.successProbability > rand)
            {
                int currentIndex = _agentRarityTemplates.IndexOf(currentAgentRarity);
                return _agentRarityTemplates[currentIndex - 1];
            }
            else
            {
                return currentAgentRarity;
            }
        }

        public AgentRarityProbabilityList GetProbabilityList()
        {
            return _probabilityList[probabilityLevel];
        }

        public bool UpgradeProbabilityLevel()
        {
            if (probabilityLevel >= _probabilityList.Count - 1) return false;
            
            probabilityLevel++;
            return true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var prob in _probabilityList)
            {
                prob.NormalizeByPriority();
            }
        }
#endif
    }

    [System.Serializable]
    public class AgentRarityProbabilityList
    {
        public int needCoin;
        [Range(0, 100)] public float myth;
        [Range(0, 100)] public float legend;
        [Range(0, 100)] public float epic;
        [Range(0, 100)] public float rare;
        [Range(0, 100), ReadOnly] public float common;

        public void NormalizeByPriority()
        {
            float total = 100f;

            myth = Mathf.Clamp(myth, 0f, total);
            total -= myth;

            legend = Mathf.Clamp(legend, 0f, total);
            total -= legend;

            epic = Mathf.Clamp(epic, 0f, total);
            total -= epic;

            rare = Mathf.Clamp(rare, 0f, total);
            total -= rare;

            common = total;
        }
    }
}