using FrameWork.UIBinding;
using UnityEngine.UI;

namespace FrameWork.VisualNovel
{
    public class UIECGController : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            ECGImage,
        }
        #endregion

        private Image _image;

        private void OnEnable()
        {
            CommandExecutor.Instance.ecgShow += ECGShow;
            CommandExecutor.Instance.ecgHide += ECGHide;
        }

        private void OnDisable()
        {
            CommandExecutor.Instance.ecgShow -= ECGShow;
            CommandExecutor.Instance.ecgHide -= ECGHide;
        }

        protected override void Initialize()
        {
            BindImage(typeof(Images));

            _image = GetImage((int)Images.ECGImage);
        }

        internal void ECGShow(string theme)
        {
            AddressableAssetManager.Instance.GetSprite(theme, (sprite) =>
            {
                _image.sprite = sprite;
                base.Show(true);
            });
        }

        internal void ECGHide()
        {
            _image.sprite = null;

            base.Hide(true);
        }
    }
}