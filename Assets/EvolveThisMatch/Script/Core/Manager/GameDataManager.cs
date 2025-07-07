using Cysharp.Threading.Tasks;
using FrameWork;
using System.Collections.Generic;
using System.Linq;
using EvolveThisMatch.Save;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class GameDataManager : PersistentSingleton<GameDataManager>
    {
        [Header("SaveData")]
        [SerializeField] private ProfileSaveDataTemplate _profileSaveData;
        [SerializeField] private FormationSaveDataTemplate _formationSaveData;

        [Header("Library")]
        [SerializeField] private AgentLibraryTemplate _agentLibrary;
        [SerializeField] private SkinLibraryTemplate _agentSkinLibrary;
        [SerializeField] private WaveLibraryTemplate _waveLibrary;
        [SerializeField] private AgentRarityLibrary _agentRarityLibrary;

        public ProfileSaveDataTemplate profileSaveData => _profileSaveData;
        public IReadOnlyList<AgentTemplate> agentTemplates => _agentLibrary.templates;
        public IReadOnlyDictionary<SkinTemplate, AgentTemplate> agentSkinTemplates => _agentSkinLibrary.templates;
        internal WaveLibraryTemplate waveLibrary => _waveLibrary;
        internal int probabilityLevel => _agentRarityLibrary.probabilityLevel;

        internal AgentTemplate GetAgentTemplateById(int id)
        {
            return _agentLibrary.templates.Where(x => x.id == id).FirstOrDefault();
        }

        internal void InitializeData()
        {
            _agentRarityLibrary.Initialize();
        }

        #region 배치 상태
        internal async UniTask<FormationSaveDataTemplate> GetFormationSaveData()
        {
            if (_formationSaveData.isLoaded == false)
            {
                await SaveManager.Instance.Load_FormationData();
            }
            return _formationSaveData;
        }

        internal async void SetFormationSaveData(List<FormationSlot> formation)
        {
            _formationSaveData.UpdateFormation(formation);

            await SaveManager.Instance.Save_FormationData();
        }
        #endregion

        #region 등급별 유닛 소환 확률
        internal AgentRarityTemplate GetAgentRandomRarity()
        {
            return _agentRarityLibrary.GetRandomAgentRarityTemplate();
        }

        internal AgentRarityTemplate GetUpgradeLimitRarity(AgentRarityTemplate currentAgentRarity)
        {
            return _agentRarityLibrary.GetUpgradeAgentRarityTemplate(currentAgentRarity);
        }

        internal AgentRarityProbabilityData GetProbabilityList()
        {
            return _agentRarityLibrary.GetProbabilityList();
        }

        internal bool UpgradeProbabilityLevel()
        {
            return _agentRarityLibrary.UpgradeProbabilityLevel();
        }
        #endregion
    }
}