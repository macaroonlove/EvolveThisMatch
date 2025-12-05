using Cysharp.Threading.Tasks;
using DG.Tweening;
using FrameWork.UIBinding;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UIPopup
{
    public class UINotificationItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Context,
        }
        #endregion

        private TextMeshProUGUI _context;
        private RectTransform _rectTransform;

        private Tween _moveTween;
        private CancellationTokenSource _cts;

        protected override void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();

            BindText(typeof(Texts));
            _context = GetText((int)Texts.Context);
        }

        internal void InitializeCTS()
        {
            _cts = new CancellationTokenSource();
        }

        internal void Show(string context)
        {
            base.Hide(true);
            _rectTransform.anchoredPosition = new Vector2(0, -20);

            _context.text = context;

            base.Show(false);
        }

        public void ToastMove(float targetY, UnityAction onComplete = null)
        {
            _moveTween?.Kill();
            _moveTween = _rectTransform.DOAnchorPosY(targetY, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public async UniTask Delay(float seconds)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: _cts.Token);
        }

        public void CancelDelay()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new CancellationTokenSource();
        }
    }
}