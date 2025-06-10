using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UnableToTargetOfAttackEffect : Effect
    {
        public override string GetDescription()
        {
            return "공격 대상이 되지 않습니다.";
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
        }

        public override int GetNumRows()
        {
            return 0;
        }
#endif
    }
}
