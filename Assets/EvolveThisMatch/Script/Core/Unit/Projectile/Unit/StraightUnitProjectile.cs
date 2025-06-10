using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class StraightUnitProjectile : UnitProjectile
    {
        [Space(10)]
        [SerializeField, Label("속도")] private float _speed;

        protected override void Move()
        {
            var targetPos = _target.projectileHitPoint.position;
            var distance = (transform.position - targetPos).sqrMagnitude;
            var moveDistance = _speed * Time.deltaTime;

            // 날라가는 중
            if (distance > moveDistance * moveDistance)
            {
                var dir = (targetPos - transform.position).normalized;
                var deltaPos = dir * moveDistance;
                transform.Translate(deltaPos);
            }
            // 충돌
            else
            {
                OnCollision();
            }
        }
    }
}