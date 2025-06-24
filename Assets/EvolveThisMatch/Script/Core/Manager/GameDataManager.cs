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

        internal AgentTemplate GetAgentTemplateById(int id)
        {
            return _agentLibrary.templates.Where(x => x.id == id).FirstOrDefault();
        }

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

        internal AgentRarityTemplate GetLimitRarity()
        {
            return _agentRarityLibrary.GetRandomAgentRarityTemplate();
        }

        internal AgentRarityTemplate GetUpgradeLimitRarity(AgentRarityTemplate currentAgentRarity)
        {
            return _agentRarityLibrary.GetUpgradeAgentRarityTemplate(currentAgentRarity);
        }
    }
}
