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
        private UnityAction<int, IReadOnlyList<GachaCost>> _action;

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

        internal void Show(int gachaCount, IReadOnlyList<GachaCost> costs, Color color, UnityAction<int, IReadOnlyList<GachaCost>> action)
        {
            _costs = costs;
            _gachaCount = gachaCount;
            _action = action;

            _background.color = color;
            _titleText.text = $"{gachaCount}회 소환";

            var sb = new StringBuilder();

            _isGachaAble = true;
            int remainCount = gachaCount;
            for (int i = 0; i < costs.Count && remainCount > 0; i++)
            {
                var cost = costs[i];

                // 최대 소환할 수 있는 개수 구하기
                int maxPickUpCount = cost.variable.Value / cost.cost;

                // 소환 불가능
                if (maxPickUpCount <= 0)
                {
                    continue;
                }

                // 해당 버튼의 최대 소환량을 넘지 않도록 제한
                int pickUpCount = Mathf.Min(remainCount, maxPickUpCount);

                // 사용 처리
                int useAmount = pickUpCount * cost.cost;
                remainCount -= pickUpCount;

                sb.Append($"<sprite name={cost.variable.IconText}> {useAmount}");

                if (remainCount > 0 && i < costs.Count - 1)
                    sb.Append("  ");
            }

            // 소환이 불가능하다면
            if (remainCount > 0)
            {
                var cost = costs[costs.Count - 1];
                sb.Clear();
                sb.Append($"<sprite name={cost.variable.IconText}> <color=red>{cost.cost * gachaCount}</color>");
                _isGachaAble = false;
            }

            _costText.text = sb.ToString();

            LayoutRebuilder.ForceRebuildLayoutImmediate(_costText.rectTransform);
            float width = Mathf.Max(160f, _costText.preferredWidth);
            (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        private void OnClick()
        {
            if (_isGachaAble)
            {
                _action?.Invoke(_gachaCount, _costs);
            }
            else
            {
                UIPopupManager.Instance.ShowConfirmPopup($"{_costs[_costs.Count - 1].variable.DisplayName}이(가) 부족합니다.");
            }
        }
    }
}