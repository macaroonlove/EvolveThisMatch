using FrameWork.UIBinding;
using TMPro;

namespace FrameWork.UI
{
    public class UIAcquisitionLocationItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Text,
        }
        #endregion

        private TextMeshProUGUI _text;

        private bool _isInitialize;

        protected override void Initialize()
        {
            if (_isInitialize) return;
            _isInitialize = true;

            BindText(typeof(Texts));

            _text = GetText((int)Texts.Text);
        }

        internal void Show(AcquisitionLocation acquisitionLocation)
        {
            Initialize();

            _text.text = acquisitionLocation.text;

            base.Show(true);
        }
    }
}