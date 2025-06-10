using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class Follow : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _offset;

        public void SetTarget(Transform target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            transform.position = _target.TransformPoint(_offset);
        }
    }
}