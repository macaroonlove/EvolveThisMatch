using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UnableToHealEffect : Effect
    {
        public override string GetDescription()
        {
            return "회복 불가";
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
