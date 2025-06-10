using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class ShaderFX : FX
    {
        public override void Play(Vector3 pos)
        {
#if UNITY_EDITOR
            Debug.LogError("특정 위치에는 실행할 수 없습니다.");
#endif
        }
    }
}
