using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITierAdvantageItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
        }
        #endregion

        [SerializeField] private Color _active; 
        [SerializeField] private Color _deActive; 

        private Image _background;

        protected override void Initialize()
        {
            BindImage(typeof(Images));

            _background = GetImage((int)Images.Background);
        }

        internal void ShowItem(bool isOn)
        {
            _background.color = isOn ? _active : _deActive;
        }
    }
}
