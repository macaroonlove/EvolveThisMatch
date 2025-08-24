using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        [SerializeField] private DepartmentSaveDataTemplate _departmentSaveData;
        [SerializeField] private DepartmentGroupTemplate _departmentGroup;

        internal DepartmentSaveDataTemplate departmentSaveData => _departmentSaveData;
        internal IReadOnlyList<DepartmentTemplate> departments => _departmentGroup.departments;

        private List<AgentTemplate> _loadedTemplates = new List<AgentTemplate>();

        protected override async void Awake()
        {
            base.Awake();

            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            List<UniTask> tasks = new List<UniTask>();

            var agents = SaveManager.Instance.agentData.ownedAgents;
            foreach (var agent in agents)
            {
                var template = GameDataManager.Instance.GetAgentTemplateById(agent.id);

                if (template != null)
                {
                    _loadedTemplates.Add(template);
                    var task = template.LoadAllSkinLobbyTemplate();
                    tasks.Add(task);
                }
            }

            await UniTask.WhenAll(tasks);
        }

        private void OnDisable()
        {
            if (_loadedTemplates.Count > 0)
            {
                foreach (var template in _loadedTemplates)
                {
                    template.ReleaseSkinLobbyTemplate();
                }
            }
            _loadedTemplates.Clear();
        }
    }
}