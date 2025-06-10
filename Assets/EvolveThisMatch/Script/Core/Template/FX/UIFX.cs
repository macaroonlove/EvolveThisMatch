using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/UIFX", fileName = "UIFX_", order = 4)]
    public class UIFX : FX
    {
        [SerializeField, Label("UI 파티클 부모")] private GameObject _uiParticle;
        [SerializeField, Label("지속 시간")] private float _duration;

        public override void Play(Unit target)
        {
            if (_uiParticle == null) return;

            Play();
        }

        public override void Play(Vector3 pos)
        {
            if (_uiParticle == null) return;

            Play();
        }

        private void Play()
        {
            var poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            var obj = poolSystem.Spawn(_uiParticle, _duration, BattleManager.Instance.canvas);

            var particle = obj.GetComponentInChildren<ParticleSystem>();
            if (particle != null)
            {
                particle.Play();
            }
        }
    }
}