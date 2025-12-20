using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UnableToAttackEffect : Effect
    {
        public override string GetDescription()
        {
            return "공격 불가";
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
