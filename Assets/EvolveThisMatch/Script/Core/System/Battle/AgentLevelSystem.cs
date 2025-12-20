using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentLevelSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private GlobalStatusTemplate _agentLevelTemplate;

        private GlobalStatusSystem _globalStatusSystem;

        public void Initialize()
        {
            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            _globalStatusSystem.ApplyGlobalStatus(_agentLevelTemplate, int.MaxValue);
        }

        public void Deinitialize()
        {
            _globalStatusSystem = null;
        }
    }
}