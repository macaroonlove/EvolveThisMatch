using DG.Tweening;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.VisualNovel
{
    public class UISCGController : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            SCGImage,
        }
        #endregion

        private Image _image;

        private List<Sequence> _sequences = new List<Sequence>();

        protected override void Initialize()
        {
            BindImage(typeof(Images));

            _image = GetImage((int)Images.SCGImage);
        }

        internal void Show(Sprite sprite, Rect position, Vector2 anchor)
        {
            var rectTransform = _image.rectTransform;

            _image.sprite = sprite;
            rectTransform.anchoredPosition = position.position;
            rectTransform.sizeDelta = position.size;
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;

            base.Show(true);
        }

        internal void Hide()
        {
            foreach (var sequence in _sequences)
            {
                sequence.Kill();
            }

            base.Hide(true);
        }

        internal void Move(Rect position, Vector2 anchor, float duration, int loopCount, Ease ease, bool isReturn, Ease returnEase)
        {
            var rectTransform = _image.rectTransform;

            Vector2 startPos = rectTransform.anchoredPosition;
            Vector2 startSize = rectTransform.sizeDelta;
            Vector2 startAnchor = rectTransform.anchorMin;
            Vector2 startPivot = rectTransform.pivot;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(rectTransform.DOAnchorPos(position.position, duration).SetEase(ease));
            sequence.Join(rectTransform.DOSizeDelta(position.size, duration).SetEase(ease));
            sequence.Join(rectTransform.DOAnchorMin(anchor, duration).SetEase(ease));
            sequence.Join(rectTransform.DOAnchorMax(anchor, duration).SetEase(ease));
            sequence.Join(rectTransform.DOPivot(anchor, duration).SetEase(ease));

            if (isReturn)
            {
                sequence.Append(rectTransform.DOAnchorPos(startPos, duration).SetEase(returnEase));
                sequence.Join(rectTransform.DOSizeDelta(startSize, duration).SetEase(returnEase));
                sequence.Join(rectTransform.DOAnchorMin(startAnchor, duration).SetEase(returnEase));
                sequence.Join(rectTransform.DOAnchorMax(startAnchor, duration).SetEase(returnEase));
                sequence.Join(rectTransform.DOPivot(startPivot, duration).SetEase(returnEase));
            }

            sequence.SetLoops(loopCount, LoopType.Restart).SetUpdate(true);

            _sequences.Add(sequence);
        }
    }
}