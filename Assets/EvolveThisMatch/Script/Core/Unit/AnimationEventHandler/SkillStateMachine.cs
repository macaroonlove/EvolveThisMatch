using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SkillStateMachine : StateMachineBehaviour
    {
        public float _executeTime = 0.7f;
        private bool _hasTriggered = false;
        private ActiveSkillAbility _activeSkillAbility;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime >= _executeTime && _hasTriggered == false)
            {
                _hasTriggered = true;
                ExecuteSkill(animator);
            }
        }

        private void ExecuteSkill(Animator animator)
        {
            if (_activeSkillAbility != null)
            {
                int SkillID = animator.GetInteger("SkillID");

                _activeSkillAbility.ExecuteSkill(SkillID);
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_activeSkillAbility == null)
            {
                _activeSkillAbility = animator.GetComponentInParent<ActiveSkillAbility>();
            }

            _hasTriggered = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _hasTriggered = false;

            _activeSkillAbility?.EndSkill();
        }
    }
}