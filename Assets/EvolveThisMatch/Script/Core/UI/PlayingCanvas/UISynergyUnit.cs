using FrameWork.Tooltip;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
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

        internal void Show(Sprite fullBody, bool isActive)
        {
            _fullBody.sprite = fullBody;
            _dim.enabled = !isActive;

            base.Show(true);
        }
    }
}