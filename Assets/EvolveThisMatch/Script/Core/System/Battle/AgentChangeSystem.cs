using EvolveThisMatch.Core;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class AgentChangeSystem : MonoBehaviour, IBattleSystem
    {
        private AgentCreateSystem _agentCreateSystem;
        private AgentReturnSystem _agentReturnSystem;

        public void Initialize()
        {
            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
            _agentReturnSystem = BattleManager.Instance.GetSubSystem<AgentReturnSystem>();
        }

        public void Deinitialize()
        {

        }

        internal ChangeAgentData? ChangeRandomUnit(AgentBattleData agentData)
        {

            var result = _agentCreateSystem.CreateUnit_Change(agentData);
            if (result.HasValue)
            {
                _agentReturnSystem.ReturnUnit_Change(agentData);

                return result;
            }

            return null;
        }
    }

    public struct ChangeAgentData
    {
        public AgentUnit agentUnit;
        public AgentTemplate agentTemplate;
        public UnityAction<AgentBattleData> action;

        public ChangeAgentData(AgentUnit agentUnit, AgentTemplate agentTemplate, UnityAction<AgentBattleData> action)
        {
            this.agentUnit = agentUnit;
            this.agentTemplate = agentTemplate;
            this.action = action;
        }
    }
}