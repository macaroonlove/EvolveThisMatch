using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UISkillPathController : UIBase
    {
        enum Images
        {
            Out,
            Center,
            In,
        }

        private Image _outImage;
        private Image _centerImage;
        private Image _inImage;

        internal void Initialize(RectTransform output, RectTransform input, int heightOffset)
        {
            BindImage(typeof(Images));

            _outImage = GetImage((int)Images.Out);
            _centerImage = GetImage((int)Images.Center);
            _inImage = GetImage((int)Images.In);

            SetPosition(output, input, heightOffset);
        }

        private void SetPosition(RectTransform output, RectTransform input, int heightOffset)
        {
            Vector2 finalOutput = output.anchoredPosition + (output.parent as RectTransform).anchoredPosition;
            Vector2 finalInput = input.anchoredPosition + (input.parent as RectTransform).anchoredPosition;

            var outRect = _outImage.rectTransform;
            var centerRect = _centerImage.rectTransform;
            var inRect = _inImage.rectTransform;

            float outputHeight = heightOffset * 0.25f;
            float inputHeight = Mathf.Abs(finalOutput.y - finalInput.y) - outputHeight;
            float width = Mathf.Abs(finalOutput.x - finalInput.x) + 4;

            Vector2 finalCenter;
            // 왼쪽으로 이어지는 선
            if (finalOutput.x > finalInput.x)
            {
                finalCenter = new Vector2(finalOutput.x - width + 4, finalOutput.y - outputHeight);
            }
            else
            {
                finalCenter = new Vector2(finalOutput.x, finalOutput.y - outputHeight);
            }

            outRect.sizeDelta = new Vector2(4, outputHeight);
            centerRect.sizeDelta = new Vector2(width, 4);
            inRect.sizeDelta = new Vector2(4, inputHeight);

            outRect.anchoredPosition = finalOutput;
            centerRect.anchoredPosition = finalCenter;
            inRect.anchoredPosition = finalInput;
            (transform as RectTransform).anchoredPosition = new Vector2(0, -73);
        }

        internal void SetColor(bool isLock)
        {
            if (isLock)
            {
                _outImage.color = Color.gray;
                _centerImage.color = Color.gray;
                _inImage.color = Color.gray;

                transform.SetAsFirstSibling();
            }
            else
            {
                _outImage.color = Color.white;
                _centerImage.color = Color.white;
                _inImage.color = Color.white;

                transform.SetAsLastSibling();
            }
        }
    }
}