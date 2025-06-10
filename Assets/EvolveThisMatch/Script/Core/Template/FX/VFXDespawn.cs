using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/VFX Despawn", fileName = "VFX Despawn_", order = 2)]
    public class VFXDespawn : FX
    {
        [SerializeField, Label("유닛에게서 삭제할 VFX")] private GameObject _vfxObj;
        
        public override void Play(Unit target)
        {
            if (target == null) return;

            target.GetAbility<FXAbility>().DespawnFX(_vfxObj);
        }

        public override void Play(Vector3 pos)
        {
#if UNITY_EDITOR
            Debug.LogError("특정 위치에는 실행할 수 없습니다.");
#endif
        }
    }
}