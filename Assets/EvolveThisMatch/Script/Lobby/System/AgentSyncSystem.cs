using EvolveThisMatch.Core;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class AgentSyncSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private GlobalStatusTemplate _syncTemplate;

        private GlobalStatusSystem _globalStatusSystem;

        public void Initialize()
        {
            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            _globalStatusSystem.ApplyGlobalStatus(_syncTemplate, int.MaxValue, null);
        }

        public void Deinitialize()
        {
            _globalStatusSystem = null;
        }
    }
}