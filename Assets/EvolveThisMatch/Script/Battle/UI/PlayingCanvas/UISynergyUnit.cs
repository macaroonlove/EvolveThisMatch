using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UISynergyUnit : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            FullBodyImage,
            Dim,
        }
        #endregion

        private Image _fullBody;
        private Image _dim;

        protected override void Initialize()
        {
            BindImage(typeof(Images));

            _fullBody = GetImage((int)Images.FullBodyImage);
            _dim = GetImage((int)Images.Dim);
        }

        internal void Show(AgentTemplate template, bool isActive)
        {
            _fullBody.sprite = template.sprite;
            _fullBody.rectTransform.anchoredPosition = template.faceCenterPosition;
            _dim.enabled = !isActive;

            base.Show(true);
        }
    }
}