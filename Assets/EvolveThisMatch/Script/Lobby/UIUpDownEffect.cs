using DG.Tweening;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIUpDownEffect : MonoBehaviour
    {
        private RectTransform _target;
        private Tween _updownTween;

        private void Start()
        {
            _target = transform as RectTransform;
            StartBreathing();
        }

        public void StartBreathing()
        {
            _updownTween = _target.DOLocalMoveY(60, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        public void StopBreathing()
        {
            _updownTween?.Kill();
            _target.localScale = Vector3.one;
        }
    }
}