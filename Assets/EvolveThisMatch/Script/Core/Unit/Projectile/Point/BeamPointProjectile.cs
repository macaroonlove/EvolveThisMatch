using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(LineRenderer))]
    public class BeamPointProjectile : PointProjectile
    {
        [SerializeField, Label("광선 유지 시간")] private float _holdTime = 0.5f;

        private float _elapsedTime;

        internal override void Initialize(Unit caster, Vector3 targetVector, UnityAction<Unit, Unit> action)
        {
            _elapsedTime = 0;

            InitializeLineRenderer(targetVector);
            InitializeCollider(targetVector);

            base.Initialize(caster, targetVector, action);
        }

        private void InitializeLineRenderer(Vector3 targetVector)
        {
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, targetVector);
        }

        private void InitializeCollider(Vector3 endPoint)
        {
            var boxCollider = GetComponent<BoxCollider>();

            Vector3 startPoint = transform.position;

            Vector3 center = (startPoint + endPoint) / 2f;

            float length = Vector3.Distance(startPoint, endPoint);

            boxCollider.size = new Vector3(length, 0.1f, 0.1f);
            boxCollider.transform.position = center;

            Vector3 direction = endPoint - startPoint;
            boxCollider.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
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