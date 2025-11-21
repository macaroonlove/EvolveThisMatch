using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class GameDataManager : PersistentSingleton<GameDataManager>
    {
        [Header("Library")]
        [SerializeField] private AgentLibraryTemplate _agentLibrary;
        [SerializeField] private List<Effect> _talentEffects = new List<Effect>();
        [SerializeField] private SkinLibraryTemplate _agentSkinLibrary;
        [SerializeField] private AgentRarityLibrary _agentRarityLibrary;

        [SerializeField] private ArtifactLibraryTemplate _artifactLibrary;
        [SerializeField] private TomeLibraryTemplate _tomeLibrary;

        public IReadOnlyList<AgentTemplate> agentTemplates => _agentLibrary.templates;
        public IReadOnlyList<Effect> talentEffects => _talentEffects;
        public IReadOnlyDictionary<SkinTemplate, AgentTemplate> agentSkinTemplates => _agentSkinLibrary.templates;
        public IReadOnlyList<AgentRarityTemplate> agentRarityTemplates => _agentRarityLibrary.agentRarityTemplates;
        public int probabilityLevel => _agentRarityLibrary.probabilityLevel;

        public IReadOnlyList<ArtifactTemplate> artifactTemplates => _artifactLibrary.templates;
        public IReadOnlyList<TomeTemplate> tomeTemplates => _tomeLibrary.templates;

        public AgentTemplate GetAgentTemplateById(int id)
        {
            return _agentLibrary.templates.Where(x => x.id == id).FirstOrDefault();
        }

        /// <summary>
        /// 게임을 시작할 때 초기화
        /// </summary>
        protected override async void Initialize()
        {
            await UniTask.WaitUntil(() => SaveManager.Instance.agentData.isLoaded);

            List<UniTask> tasks = new List<UniTask>();
            
            var agents = SaveManager.Instance.agentData.ownedAgents;
            foreach (var agent in agents)
            {
                var template = GetAgentTemplateById(agent.id);

                if (template != null)
                {
                    var task = template.LoadSkinBattleTemplate();
                    tasks.Add(task);
                }
            }

            await UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// 전투가 시작할 때 마다 초기화
        /// </summary>
        internal void InitializeData()
        {
            _agentRarityLibrary.Initialize();
        }

        #region 등급별 유닛 소환 확률
        public AgentRarityTemplate GetAgentRandomRarity()
        {
            return _agentRarityLibrary.GetRandomAgentRarityTemplate();
        }

        public AgentRarityTemplate GetUpgradeLimitRarity(AgentRarityTemplate currentAgentRarity)
        {
            return _agentRarityLibrary.GetUpgradeAgentRarityTemplate(currentAgentRarity);
        }

        public AgentRarityProbabilityData GetProbabilityList()
        {
            return _agentRarityLibrary.GetProbabilityList();
        }

        public bool UpgradeProbabilityLevel()
        {
            return _agentRarityLibrary.UpgradeProbabilityLevel();
        }
        #endregion
    }
}