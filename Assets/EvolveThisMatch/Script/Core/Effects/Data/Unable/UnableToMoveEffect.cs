using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UnableToMoveEffect : Effect
    {
        public override string GetDescription()
        {
            return "이동 불가";
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
