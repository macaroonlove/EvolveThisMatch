using DG.Tweening;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Battle
{
    public class UIRewardItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Text
        }
        #endregion

        private TextMeshProUGUI _text;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _text = GetText((int)Texts.Text);
            transform.localScale = Vector3.zero;
        }

        internal void Show(ObscuredIntVariable variable, int count, UnityAction action)
        {
            _text.text = $"- <size=44><sprite name={variable.IconText}></size> {variable.DisplayName} x{count}";

            transform.DOScale(1, 0.5f).OnComplete(() => { action?.Invoke(); });
        }
    }
}