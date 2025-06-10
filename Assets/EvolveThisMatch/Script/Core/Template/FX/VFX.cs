using FrameWork.Editor;
using System.Collections;
using UnityEngine;
using VInspector;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/VFX", fileName = "VFX_", order = 1)]
    public class VFX : FX
    {
        [SerializeField, Label("파티클인가?")] private bool _isParticle = true;
        [SerializeField, Label("VFX")] private GameObject _vfxObj;
        [SerializeField, Label("생성 위치")] private ESpawnPoint _spawnPoint;
        
        [Space(10)]
        [SerializeField, Label("무한지속 여부")] private bool _isInfinity;
        [HideIf("_isInfinity")]
        [SerializeField, Label("지속 시간")] private float _duration;
        [EndIf]

        [Space(10)]
        [Tooltip("유닛을 대상으로 하였을 때만 적용")]
        [SerializeField, Label("반복 생성 여부")] private bool _isRepeat;
        [ShowIf("_isRepeat")]
        [SerializeField, Label("생성 사이클")] private float _cycle;
        [EndIf]

        [Space(10)]
        [SerializeField, Label("위치 오프셋")] private Vector3 _posOffset;
        [SerializeField, Label("회전 오프셋")] private Vector3 _rotOffset;

        [Space(10)]
        [Tooltip("타겟 방식으로 보내야만 사용 가능")]
        [SerializeField, Label("적을 따라갈지 여부")] private bool _isFollowTarget;

        public override void Play(Unit target)
        {
            if (target == null) return;

            var fxAbility = target.GetAbility<FXAbility>();

            if (_isRepeat)
            {
                var coroutine = fxAbility.StartCoroutine(CoPlay(target, fxAbility));
                fxAbility.AddCoroutineFX(_vfxObj, coroutine);
            }
            else
            {
                DoPlay(target, fxAbility);
            }            
        }

        private IEnumerator CoPlay(Unit target, FXAbility fxAbility)
        {
            var wfs = new WaitForSeconds(_cycle);

            while (true)
            {
                DoPlay(target, fxAbility);
                yield return wfs;
            }
        }

        private void DoPlay(Unit target, FXAbility fxAbility)
        {
            Transform point = GetSpawnPoint(target);

            Vector3 pos = point.TransformPoint(_posOffset);

            Quaternion baseRot = point.rotation;
            Quaternion rot = baseRot * Quaternion.Euler(_rotOffset);

            var obj = Play(pos, rot);

            if (_isFollowTarget)
            {
                Follow follow = obj.gameObject.GetComponent<Follow>();
                if (follow == null)
                {
                    follow = obj.gameObject.AddComponent<Follow>();
                }
                follow.SetTarget(point, _posOffset);
            }

            fxAbility.AddFX(obj);
        }

        public override void Play(Vector3 pos)
        {
            Quaternion rot = Quaternion.Euler(_rotOffset);

            Play(pos, rot);
        }

        private GameObject Play(Vector3 pos, Quaternion rot)
        {
            var poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            
            GameObject obj;
            if (_isInfinity)
            {
                obj = poolSystem.Spawn(_vfxObj);
            }
            else
            {
                obj = poolSystem.Spawn(_vfxObj, _duration);
            }

            pos += _posOffset;

            obj.transform.SetPositionAndRotation(pos, rot);

            if (_isParticle)
            {
                var particle = obj.GetComponent<ParticleSystem>();
                if (particle != null)
                {
                    particle.Play();
                }
            }

            return obj;
        }

        private Transform GetSpawnPoint(Unit target)
        {
            var point = target.transform;

            switch (_spawnPoint)
            {
                case ESpawnPoint.Head:
                    if (target.headPoint != null)
                    {
                        point = target.headPoint;
                    }
                    break;
                case ESpawnPoint.Body:
                    if (target.bodyPoint != null)
                    {
                        point = target.bodyPoint;
                    }
                    break;
                case ESpawnPoint.LeftHand:
                    if (target.leftHandPoint != null)
                    {
                        point = target.leftHandPoint;
                    }
                    break;
                case ESpawnPoint.RightHand:
                    if (target.rightHandPoint != null)
                    {
                        point = target.rightHandPoint;
                    }
                    break;
                case ESpawnPoint.Foot:
                    if (target.footPoint != null)
                    {
                        point = target.footPoint;
                    }
                    break;
                case ESpawnPoint.ProjectileHit:
                    if (target.projectileHitPoint != null)
                    {
                        point = target.projectileHitPoint;
                    }
                    break;
            }

            return point;
        }
    }
}