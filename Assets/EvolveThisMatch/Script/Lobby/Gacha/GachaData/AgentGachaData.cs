using Cysharp.Threading.Tasks;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class AgentGachaData : GachaData
    {
        [Serializable]
        public class PickUpData
        {
            public AgentTemplate template;
            public float rarity;
        }

        [Header("아군 유닛 등급별 확률정보")]
        [SerializeField, Range(0, 100.0f)] private int _mythRarity;
        [SerializeField, Range(0, 100.0f)] private int _legendRarity;
        [SerializeField, Range(0, 100.0f)] private int _epicRarity;
        [SerializeField, Range(0, 100.0f)] private int _rareRarity;
        [SerializeField, Range(0, 100.0f)] private int _commonRarity;

        [Header("픽업 유닛"), Tooltip("픽업 유닛은 등급별 확률에서 제외됩니다.")]
        [SerializeField] private List<PickUpData> _pickupList = new List<PickUpData>();

        private float[] _rarityProbabilities;
        private List<AgentTemplate> _mythAgents;
        private List<AgentTemplate> _legendAgents;
        private List<AgentTemplate> _epicAgents;
        private List<AgentTemplate> _rareAgents;
        private List<AgentTemplate> _commonAgents;

        private List<AgentTemplate> _gachaList = new List<AgentTemplate>();
        private List<PickUpData> _mythPickUpList = new List<PickUpData>();

        internal override async void Initialize(UIGachaResultCanvas gachaResultCanvas)
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            base.Initialize(gachaResultCanvas);

            _rarityProbabilities = new float[4];
            _rarityProbabilities[0] = _legendRarity;
            _rarityProbabilities[1] = _epicRarity;
            _rarityProbabilities[2] = _rareRarity;
            _rarityProbabilities[3] = _commonRarity;

            // 픽업 유닛을 제외한 아군 유닛 리스트
            var agentList = GameDataManager.Instance.agentTemplates.Where(x => !_pickupList.Any(y => y.template == x));

            _mythPickUpList = _pickupList.Where(p => p.template.rarity.rarity == EAgentRarity.Myth).ToList();
            _mythAgents = agentList.Where(template => template.rarity.rarity == EAgentRarity.Myth).ToList();
            _legendAgents = agentList.Where(template => template.rarity.rarity == EAgentRarity.Legend).ToList();
            _epicAgents = agentList.Where(template => template.rarity.rarity == EAgentRarity.Epic).ToList();
            _rareAgents = agentList.Where(template => template.rarity.rarity == EAgentRarity.Rare).ToList();
            _commonAgents = agentList.Where(template => template.rarity.rarity == EAgentRarity.Common).ToList();
        }

        internal override void PickUp(int gachaCount)
        {
            _gachaList.Clear();

            for (int i = 0; i < gachaCount; i++)
            {
                var agent = GetRandomAgent();
                _gachaList.Add(agent);
                GameDataManager.Instance.profileSaveData.AddAgent(agent.id);
            }

            _ = SaveManager.Instance.Save_ProfileData();

            _gachaResultCanvas.Show(_gachaList);
        }

        private AgentTemplate GetRandomAgent()
        {
            int rarityIndex;

            if (_confirmedPickUpVariable != null)
            {
                if (_confirmedPickUpVariable.Value > 0)
                {
                    _confirmedPickUpVariable.AddValue(-1);

                    var agentTemplate = GetPickUp(_pickupList, 100);

                    if (agentTemplate != null) return agentTemplate;

                    rarityIndex = GetRandomRarityIndex();
                }
                else
                {
                    _confirmedPickUpVariable.Value = 50;

                    var agentTemplate = GetPickUp(_mythPickUpList, 100);

                    if (agentTemplate != null) return agentTemplate;

                    rarityIndex = 0;
                }
            }
            else
            {
                var agentTemplate = GetPickUp(_pickupList, 100);

                if (agentTemplate != null) return agentTemplate;

                rarityIndex = GetRandomRarityIndex();
            }

            _additionalVariable?.AddValue(1);

            // 일반 유닛 뽑기
            List<AgentTemplate> agents;
            switch (rarityIndex)
            {
                case 0: agents = _mythAgents; break;
                case 1: agents = _legendAgents; break;
                case 2: agents = _epicAgents; break;
                case 3: agents = _rareAgents; break;
                default: agents = _commonAgents; break;
            }

            var index = Random.Range(0, agents.Count);
            return agents[index];
        }

        private AgentTemplate GetPickUp(List<PickUpData> pickUpList, float maxProbability)
        {
            // 픽업 유닛 뽑기
            float rand = Random.Range(0, maxProbability);
            float cumulativeProbability = 0;

            foreach (var pickUp in pickUpList)
            {
                cumulativeProbability += pickUp.rarity;
                if (rand <= cumulativeProbability)
                {
                    return pickUp.template;
                }
            }

            return null;
        }

        private int GetRandomRarityIndex()
        {
            float rand = Random.Range(0, 100.0f);
            float cumulativeProbability = 0;

            for (int i = 0; i < _rarityProbabilities.Length; i++)
            {
                cumulativeProbability += _rarityProbabilities[i];
                if (rand <= cumulativeProbability)
                {
                    return i;
                }
            }

            return _rarityProbabilities.Length - 1;
        }
    }
}