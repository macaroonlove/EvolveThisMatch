using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DeployAbility : ConditionAbility
    {
        [SerializeField, ReadOnly] private bool _isSortie;

        internal override void Initialize(Unit unit)
        {
            if (unit is AgentUnit agentUnit && agentUnit.template.job.job != EJob.Melee)
            {
                _isSortie = true;
            }
            else
            {
                _isSortie = false;
            }

            base.Initialize(unit);
        }

        internal override bool IsExecute()
        {
            return !_isSortie;
        }

        internal void Sortie()
        {
            _isSortie = true;
            unit.ReleaseCurrentAbility();
        }
    }
}