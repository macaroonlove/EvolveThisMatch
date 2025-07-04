using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DeployAbility : ConditionAbility
    {
        [SerializeField, ReadOnly, Label("성 유닛인가?")] private bool _isCastle;
        [SerializeField, ReadOnly, Label("출격하였는가?")] private bool _isSortie;
        private MoveAbility _moveAbility;

        internal bool isSortie => _isSortie;

        internal override void Initialize(Unit unit)
        {
            if (unit is AgentUnit agentUnit && agentUnit.template.job.job != EJob.Melee)
            {
                _isCastle = true;
            }
            else
            {
                _isCastle = false;
                _isSortie = false;
                _moveAbility = unit.GetAbility<MoveChaseAbility>();
            }

            base.Initialize(unit);
        }

        internal override bool IsExecute()
        {
            if (_isCastle) return false;

            return !_isSortie;
        }

        internal void Sortie(bool isOn)
        {
            _isSortie = isOn;

            unit.ReleaseCurrentAbility();
            _moveAbility.StopAbility();
        }
    }
}