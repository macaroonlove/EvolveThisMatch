using UnityEngine;

namespace FrameWork
{
    public static class AnimatorExtensions
    {
        public static bool TrySetTrigger(this Animator animator, int parameterHash)
        {
            foreach (var param in animator.parameters)
            {
                if (param.nameHash == parameterHash)
                {
                    animator.SetTrigger(parameterHash);
                    return true;
                }
            }

            Debug.LogWarning($"해당 애니메이션 파라미터가 존재하지 않습니다 {parameterHash}");
            return false;
        }
    }
}