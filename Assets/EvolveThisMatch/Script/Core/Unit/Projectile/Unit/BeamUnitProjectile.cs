using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    [RequireComponent(typeof(LineRenderer))]
    public class BeamUnitProjectile : UnitProjectile
    {
        [SerializeField, Label("광선 유지 시간")] private float _holdTime = 0.5f;

        private LineRenderer _lineRenderer;
        private float _elapsedTime;

        internal override void Initialize(Unit caster, Unit target, UnityAction<Unit, Unit> action)
        {
            _elapsedTime = 0;

            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;

            _lineRenderer.SetPosition(0, caster.transform.position);
            _lineRenderer.SetPosition(1, target.projectileHitPoint.position);

            base.Initialize(caster, target, action);

            OnCollision();
        }

        protected override void Move()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _holdTime)
            {
                DeSpawn();
            }
        }
    }
}