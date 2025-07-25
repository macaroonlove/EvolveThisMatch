using DG.Tweening;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UIRewardItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Text
        }
        enum Images
        {
            Icon,
        }
        #endregion

        private TextMeshProUGUI _text;
        private Image _icon;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _text = GetText((int)Texts.Text);
            _icon = GetImage((int)Images.Icon);

            transform.localScale = Vector3.zero;
        }

        internal void Show(Sprite icon, int count, UnityAction action)
        {
            _icon.sprite = icon;
            _text.text = count.ToString();

            transform.DOScale(1, 0.5f).OnComplete(() => { action?.Invoke(); });
        }
    }
}