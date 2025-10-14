using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using System.Collections.Generic;

namespace EvolveThisMatch.Lobby
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        private List<AgentTemplate> _loadedTemplates = new List<AgentTemplate>();

        protected override async void Initialize()
        {
            await UniTask.WaitUntil(() => SaveManager.Instance.agentData.isLoaded);

            var agents = SaveManager.Instance.agentData.ownedAgents;
            foreach (var agent in agents)
            {
                var template = GameDataManager.Instance.GetAgentTemplateById(agent.id);

                if (template != null)
                {
                    _loadedTemplates.Add(template);
                    template.LoadAllSkinLobbyTemplate().Forget();
                }
            }
        }

        private void OnDestroy()
        {
            if (_loadedTemplates.Count > 0)
            {
                foreach (var template in _loadedTemplates)
                {
                    template.ReleaseSkinLobbyTemplate();
                }
            }
            _loadedTemplates.Clear();

            BattleManager.Instance.DeinitializeBattle();
        }
    }
}