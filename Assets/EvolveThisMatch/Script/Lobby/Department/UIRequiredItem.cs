using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIRequiredItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Icon,
        }
        enum Texts
        {
            NeedCount,
        }
        #endregion

        private Image _icon;
        private TextMeshProUGUI _needCount;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _icon = GetImage((int)Images.Icon);
            _needCount = GetText((int)Texts.NeedCount);
        }

        internal async void Show(RequiredItem requiredItem)
        {
            var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(requiredItem.Variable);

            _icon.sprite = variable.Icon;
            _needCount.text = requiredItem.Amount.ToString();

            base.Show(true);
        }
    }
}