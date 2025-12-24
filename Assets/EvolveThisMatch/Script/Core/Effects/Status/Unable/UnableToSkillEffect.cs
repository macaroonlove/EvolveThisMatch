using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UnableToSkillEffect : Effect
    {
        public override string GetDescription()
        {
            return "스킬 사용 불가";
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
