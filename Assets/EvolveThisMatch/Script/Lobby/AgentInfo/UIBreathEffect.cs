using DG.Tweening;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIBreathEffect : MonoBehaviour
    {
        private RectTransform _target;
        private Tween _breathTween;

        private void Start()
        {
            _target = transform as RectTransform;
            StartBreathing();
        }

        public void StartBreathing()
        {
            _breathTween = _target.DOScaleY(1.01f, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        public void StopBreathing()
        {
            _breathTween?.Kill();
            _target.localScale = Vector3.one;
        }
    }
}