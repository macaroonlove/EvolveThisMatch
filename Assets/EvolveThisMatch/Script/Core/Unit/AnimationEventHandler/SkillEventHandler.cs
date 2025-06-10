using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class SkillEventHandler : MonoBehaviour
    {
        internal UnityAction onSkill;
        internal UnityAction onSkillEnd;

        public void SkillEvent()
        {
            onSkill?.Invoke();
        }

        public void SkillEndEvent()
        {
            onSkillEnd?.Invoke();
        }
    }
}