using DG.Tweening;
using UnityEngine;

namespace EvolveThisMatch.Login
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIBlinkEffect : MonoBehaviour
    {
        [Header("Blink Settings")]
        [SerializeField] private float _minAlpha = 0.3f;
        [SerializeField] private float _maxAlpha = 1f;
        [SerializeField] private float _fadeDuration = 1.4f;
        [SerializeField] private float _idleDelay = 0.2f;

        private CanvasGroup _canvasGroup;
        private Tween _blinkTween;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            PlayBlink();
        }

        private void OnDisable()
        {
            _blinkTween?.Kill();
        }

        private void PlayBlink()
        {
            _canvasGroup.alpha = _maxAlpha;

            _blinkTween = _canvasGroup
                .DOFade(_minAlpha, _fadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(_idleDelay);
        }
    }
}