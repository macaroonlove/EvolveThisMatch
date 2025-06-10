using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ArcUnitProjectile : UnitProjectile
    {
        [Space(10)]
        [SerializeField, Label("포물선 최대 높이")] private float _height;
        [SerializeField, Label("속도")] private float _speed;

        private Vector3 _startPosition;
        private float _totalMoveTime;
        private float _elapsedTime;

        protected void OnEnable()
        {
            _startPosition = transform.position;
            _totalMoveTime = CalculateTotalMoveTime();
            _elapsedTime = 0f;
        }

        /// <summary>
        /// 총 이동 시간 계산
        /// </summary>
        private float CalculateTotalMoveTime()
        {
            float distance = Vector3.Distance(_startPosition, _target.projectileHitPoint.position);
            return distance / _speed;
        }

        protected override void Move()
        {
            _elapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(_elapsedTime / _totalMoveTime);

            Vector3 targetPos = _target.projectileHitPoint.position;

            // 수평 이동
            Vector3 horizontalPos = Vector3.Lerp(_startPosition, targetPos, time);

            // 수직 이동
            float verticalOffset = Mathf.Sin(time * Mathf.PI) * _height;

            transform.position = new Vector3(horizontalPos.x, horizontalPos.y + verticalOffset, horizontalPos.z);

            // 목표 위치 도달 시 충돌 처리
            if (time >= 1f)
            {
                OnCollision();
            }
        }
    }
}