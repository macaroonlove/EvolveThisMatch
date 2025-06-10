using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class ArcPointProjectile : PointProjectile
    {
        [Space(10)]
        [SerializeField, Label("포물선 최대 높이")] private float _height;
        [SerializeField, Label("속도")] private float _speed;

        private Vector3 _startPosition;
        private float _totalMoveTime;
        private float _elapsedTime;

        internal override void Initialize(Unit caster, Vector3 targetVector, UnityAction<Unit, Unit> action)
        {
            _startPosition = transform.position;
            _totalMoveTime = CalculateTotalMoveTime();
            _elapsedTime = 0f;

            base.Initialize(caster, targetVector, action);
        }

        /// <summary>
        /// 총 이동 시간 계산
        /// </summary>
        private float CalculateTotalMoveTime()
        {
            float distance = Vector3.Distance(_startPosition, _targetVector);
            return distance / _speed;
        }

        protected override void Move()
        {
            _elapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(_elapsedTime / _totalMoveTime);

            // 수평 이동
            Vector3 horizontalPos = Vector3.Lerp(_startPosition, _targetVector, time);

            // 수직 이동
            float verticalOffset = Mathf.Sin(time * Mathf.PI) * _height;

            transform.position = new Vector3(horizontalPos.x, horizontalPos.y + verticalOffset, horizontalPos.z);
        }
    }
}