using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITomeInfoCanvas : UIBase
    {
        #region 바인딩
        enum Texts
        {
            DisplayName,
            Description,
            NeedCoinValue,
            CooldownTimeValue,
            RangeValue,
        }
        enum Images
        {
            Icon,
            VideoImage,
        }
        enum CanvasGroups
        {
            Info,
        }
        #endregion

        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _description;
        private TextMeshProUGUI _needCoinValue;
        private TextMeshProUGUI _cooldownTimeValue;
        private TextMeshProUGUI _rangeValue;
        private Image _icon;
        private Image _videoImage;
        private CanvasGroupController _info;

        private UnityAction _action;

        internal void Initialize(UnityAction action = null)
        {
            _action = action;

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindCanvasGroupController(typeof(CanvasGroups));

            _displayName = GetText((int)Texts.DisplayName);
            _description = GetText((int)Texts.Description);
            _needCoinValue = GetText((int)Texts.NeedCoinValue);
            _cooldownTimeValue = GetText((int)Texts.CooldownTimeValue);
            _rangeValue = GetText((int)Texts.RangeValue);
            _icon = GetImage((int)Images.Icon);
            _videoImage = GetImage((int)Images.VideoImage);
            _info = GetCanvasGroupController((int)CanvasGroups.Info);
        }

        internal void Show(TomeTemplate template, ItemSaveData.Tome owned)
        {
            if (template == null)
            {
                _info.Hide(true);
                return;
            }

            _displayName.text = template.displayName;
            _needCoinValue.text = $"{template.needCoin} 개";
            _cooldownTimeValue.text = $"{template.cooldownTime} 초";
            _icon.sprite = template.sprite;

            if (template.rangeType == ETomeRangeType.All)
            {
                _rangeValue.text = "전체";
            }
            else if (template.rangeType == ETomeRangeType.Circle)
            {
                _rangeValue.text = $"원 ({template.range})";
            }

            _description.text = template.description.Replace("{value}", $"{template.initValue + owned.level - 1}");

            _info.Show(true);
        }
    }
}