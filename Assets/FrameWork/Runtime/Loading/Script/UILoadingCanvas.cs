using FrameWork.UIBinding;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.Loading
{
    public class UILoadingCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
            Bar,
        }
        #endregion

        private Image _background;
        private Image _bar;

        protected override void Initialize()
        {
            BindImage(typeof(Images));

            _background = GetImage((int)Images.Background);
            _bar = GetImage((int)Images.Bar);
        }

        internal void Show(string sceneName, UnityAction<string> onShow)
        {
            AddressableAssetManager.Instance.GetSprite($"{sceneName}.PNG", sprite =>
            {
                base.Show(true);
                if (sprite != null)
                {
                    _background.sprite = sprite;
                }

                onShow?.Invoke(sceneName);
            });
        }

        public override void Hide(bool isForce = false)
        {
            base.Hide(isForce);

            SetProgress(0);
        }

        internal void SetProgress(float progress)
        {
            _bar.fillAmount = progress;
        }
    }
}