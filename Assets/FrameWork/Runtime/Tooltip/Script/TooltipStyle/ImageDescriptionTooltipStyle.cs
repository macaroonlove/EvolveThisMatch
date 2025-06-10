using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.Tooltip
{
    public class ImageDescriptionTooltipStyle : TooltipStyle
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Description
        }

        enum Images
        {
            AssistanceImage
        }
        #endregion

        private TextMeshProUGUI _descriptionText;
        private Image _assistanceImage;

#if UNITY_EDITOR
        internal override TooltipData CreateField()
        {
            var data = new TooltipData();

            data.AddString("Description", "");
            data.AddSprite("AssistanceImage", null);
            
            return data;
        }
#endif

        internal override async void ApplyData(TooltipData data)
        {
            _descriptionText.text = data.GetString("Description");
            _assistanceImage.sprite = data.GetSprite("AssistanceImage");

            await UniTask.Yield();

            var newHeight = _descriptionText.textInfo.lineCount * 50;
            Vector2 sizeDelta = _descriptionText.rectTransform.sizeDelta;
            sizeDelta.y = newHeight;
            _descriptionText.rectTransform.sizeDelta = sizeDelta;
        }

        protected override void Initialize()
        {
            base.Initialize();

            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _descriptionText = GetText((int)Texts.Description);
            _assistanceImage = GetImage((int)Images.AssistanceImage);
        }
    }
}