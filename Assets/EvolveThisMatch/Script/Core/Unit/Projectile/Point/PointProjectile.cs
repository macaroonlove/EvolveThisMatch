using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace EvolveThisMatch.Core
{
    public abstract class PointProjectile : Projectile
    {
        [Space(10)]
        [SerializeField, Label("충돌 대상")] private LayerMask _layerMask;
        [SerializeField, Label("관통 여부")] private bool _isPiercing;

        [ShowIf("_isPiercing")]
        [SerializeField, Label("무한 관통 여부")] private bool _isInfinityPiercing;
        [HideIf("_isInfinityPiercing")]
        [SerializeField, Label("관통 개수")] private int _piercingCount;
        [EndIf]

        protected Unit _caster;
        protected Vector3 _targetVector;
        protected int _collisionCount;

        internal virtual void Initialize(Unit caster, Vector3 targetVector, UnityAction<Unit, Unit> action)
        {
            _caster = caster;
            _targetVector = targetVector;
            _action = action;

            ExecuteCasterFX(caster);

            if (_isLookTarget)
            {
                transform.GetChild(0).LookAt(_targetVector);
            }

            _isInit = true;
            _collisionCount = 0;
        }

        private void Update()
        {
            if (_isInit == false) return;

            Move();

            // 최종 위치에 도달했다면
            if ((transform.position - _targetVector).sqrMagnitude < 0.01f)
            {
                DeSpawn();
            }
        }

        protected abstract void Move();

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _layerMask) != 0)
            {
                if (other.TryGetComponent(out Unit targetUnit))
                {
                    OnCollision(targetUnit);
                }
            }
        }

        private void OnCollision(Unit target)
        {
            if (_isSplash)
            {
                var targets = _getTargetData.GetTarget(target);

                for (int i = 0; i < targets.Count; i++)
                {
                    _action?.Invoke(_caster, targets[i]);
                }
            }
            else
            {
                _action?.Invoke(_caster, target);
            }

            ExecuteTargetFX(target);

            // 관통되는 투사체가 아니라면
            if (_isPiercing == false)
            {
                DeSpawn();
            }
            // 관통되는 투사체인데, 무한 관통이 아니라면
            else if (_isInfinityPiercing == false)
            {
                _collisionCount++;

                if (_collisionCount >= _piercingCount)
                {
                    DeSpawn();
                }
            }
        }
    }
}