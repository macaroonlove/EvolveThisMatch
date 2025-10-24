using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaButton : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Title,
            CostText,
        }

        enum Images
        {
            Background,
        }
        #endregion

        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _costText;
        private Image _background;

        private bool _isGachaAble;
        private int _gachaCount;
        private IReadOnlyList<GachaCost> _costs;
        private UnityAction<int> _action;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _titleText = GetText((int)Texts.Title);
            _costText = GetText((int)Texts.CostText);
            _background = GetImage((int)Images.Background);

            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(OnClick);
            }
        }

        internal async void Show(int gachaCount, List<GachaCost> costs, string colorString, UnityAction<int> action)
        {
            _costs = costs;
            _gachaCount = gachaCount;
            _action = action;

            _titleText.text = $"{gachaCount}회 소환";

            if (ColorUtility.TryParseHtmlString(colorString, out Color color))
            {
                _background.color = color;
            }

            var sb = new StringBuilder();

            _isGachaAble = true;
            int remainCount = gachaCount;

            for (int i = 0; i < costs.Count && remainCount > 0; i++)
            {
                var cost = costs[i];

                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(cost.costVariable);

                if (variable != null)
                {
                    // 최대 소환할 수 있는 개수 구하기
                    int maxPickUpCount = variable.Value / cost.price;

                    // 소환이 가능하다면
                    if (maxPickUpCount > 0)
                    {
                        // 해당 버튼의 최대 소환량을 넘지 않도록 제한
                        int pickUpCount = Mathf.Min(remainCount, maxPickUpCount);

                        // 사용 처리
                        int useAmount = pickUpCount * cost.price;
                        remainCount -= pickUpCount;

                        sb.Append($"<sprite name={variable.IconText}> {useAmount}");

                        if (remainCount > 0 && i < costs.Count - 1)
                            sb.Append("  ");
                    }
                }
            }

            // 소환이 불가능하다면
            if (remainCount > 0)
            {
                var cost = costs[costs.Count - 1];
                sb.Clear();

                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(cost.costVariable);

                if (variable != null)
                {
                    sb.Append($"<sprite name={variable.IconText}> <color=red>{cost.price * gachaCount}</color>");
                }
                _isGachaAble = false;
            }

            _costText.text = sb.ToString();

            LayoutRebuilder.ForceRebuildLayoutImmediate(_costText.rectTransform);
            float width = Mathf.Max(160f, _costText.preferredWidth);
            (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            gameObject.SetActive(true);
        }

        public override void Hide(bool isForce = false)
        {
            gameObject.SetActive(false);
        }

        private async void OnClick()
        {
            if (_isGachaAble)
            {
                _action?.Invoke(_gachaCount);
            }
            else
            {
                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(_costs[_costs.Count - 1].costVariable);
                // 해당 ObscuredIntVariable을 찾을 수 있다면
                if (variable != null)
                {
                    UIPopupManager.Instance.ShowConfirmPopup($"{variable.DisplayName}이(가) 부족합니다.");
                }
            }
        }
    }
}