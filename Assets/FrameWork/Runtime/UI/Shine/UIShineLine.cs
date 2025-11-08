using DG.Tweening;
using UnityEngine;

namespace FrameWork.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIShineLine : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _rotate;
        [SerializeField] private float _delay;

        private RectTransform _shineRect;
        private RectTransform _textRect;
        private Sequence _seq;

        private void Awake()
        {
            _shineRect = GetComponent<RectTransform>();
            _textRect = GetComponentInParent<RectTransform>();

            // 각도 적용
            _shineRect.localRotation = Quaternion.Euler(0, 0, _rotate);
        }

        private void OnEnable()
        {
            StartShine();
        }

        private void OnDisable()
        {
            StopShine();
        }

        public void StartShine()
        {
            StopShine();

            Vector2 dir = _shineRect.right;
            float length = _textRect.rect.width * 5;

            Vector2 startPos = -dir * length;
            Vector2 endPos = dir * length;

            _shineRect.anchoredPosition = startPos;

            _seq = DOTween.Sequence()
                .Append(_shineRect.DOAnchorPos(endPos, _speed).SetEase(Ease.Linear))
                .AppendInterval(_delay)
                .SetLoops(-1, LoopType.Restart);
        }

        public void StopShine()
        {
            if (_seq != null && _seq.IsActive())
            {
                _seq.Kill();
                _seq = null;
            }
        }
    }
}
