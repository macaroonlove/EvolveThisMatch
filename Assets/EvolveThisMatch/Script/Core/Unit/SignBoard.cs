using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SignBoard : AllyUnit
    {
        private TextMeshProUGUI _sortieName;

        internal AgentUnit linkUnit { get; private set; }

        private void Awake()
        {
            _sortieName = GetComponentInChildren<TextMeshProUGUI>();
        }

        internal void Initialize(AgentUnit agentUnit)
        {
            linkUnit = agentUnit;
            _sortieName.text = agentUnit.template.displayName;

            base.Initialize(this);
        }
    }
}