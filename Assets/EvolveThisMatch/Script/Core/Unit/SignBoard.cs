using TMPro;

namespace EvolveThisMatch.Core
{
    public class SignBoard : AllyUnit
    {
        private TextMeshProUGUI _sortieName;

        public AgentUnit linkUnit { get; private set; }

        private void Awake()
        {
            _sortieName = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Initialize(AgentUnit agentUnit)
        {
            linkUnit = agentUnit;
            _sortieName.text = agentUnit.template.displayName;

            base.Initialize(this);
        }
    }
}