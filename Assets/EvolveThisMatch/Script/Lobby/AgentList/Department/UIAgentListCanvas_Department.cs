using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentListCanvas_Department : UIAgentListCanvas
    {
        internal override void Initialize(UnityAction<AgentTemplate, AgentSaveData.Agent> action = null)
        {
            base.Initialize(action);
        }

        private new void Start()
        {
            base.Start();

            ChangeFilterOrder(2);

            _agentListItems[0].SelectItem();
        }
    }
}