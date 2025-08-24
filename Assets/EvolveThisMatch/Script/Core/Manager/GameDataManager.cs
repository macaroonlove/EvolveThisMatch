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
        [SerializeField] private BattleDataTemplate _battleData;
        [SerializeField] private AgentLibraryTemplate _agentLibrary;
        [SerializeField] private SkinLibraryTemplate _agentSkinLibrary;
        [SerializeField] private AgentRarityLibrary _agentRarityLibrary;
        [SerializeField] private AgentTalentLibrary _agentTalentLibrary;

        [SerializeField] private ArtifactLibraryTemplate _artifactLibrary;
        [SerializeField] private TomeLibraryTemplate _tomeLibrary;

        public BattleDataTemplate battleData => _battleData;

        public IReadOnlyList<AgentTemplate> agentTemplates => _agentLibrary.templates;
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
        protected override void Initialize()
        {
            _agentTalentLibrary.Initialize();
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

        #region 유닛의 재능 불러오기 및 설정
        public List<AgentTalentData> GetAllTalentEffect()
        {
            return _agentTalentLibrary.GetAllTalentEffect();
        }

        public AgentTalentData GetTalentEffect(int id)
        {
            return _agentTalentLibrary.GetTalentEffect(id);
        }

        public AgentTalentData GetRandomTalentEffect()
        {
            return _agentTalentLibrary.GetRandomTalentEffect();
        }

        public AgentRarityTemplate GetRandomRarity()
        {
            return _agentTalentLibrary.GetRandomRarity();
        }
        #endregion
    }
}