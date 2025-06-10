using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AbnormalStatusTestCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            StatusApplyButton,
            PrevButton,
            NextButton,
        }

        enum Texts
        {
            StatusName,
        }
        #endregion

        [SerializeField] private List<AbnormalStatusTemplate> _abnormalStatusList = new List<AbnormalStatusTemplate>();

        private Unit _unit;
        private AbnormalStatusTemplate _template;
        private TextMeshProUGUI _statusNameText;
        private int _index = 0;

        protected override void Awake()
        {
            base.Awake();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            GetButton((int)Buttons.StatusApplyButton).onClick.AddListener(ApplyStatus);
            GetButton((int)Buttons.PrevButton).onClick.AddListener(PrevButton);
            GetButton((int)Buttons.NextButton).onClick.AddListener(NextButton);
            _statusNameText = GetText((int)Texts.StatusName);
        }

        internal void Initialize(Unit unit)
        {
            _unit = unit;
            
            UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            _template = _abnormalStatusList[_index];
            _statusNameText.text = _template.displayName;
        }

        private void PrevButton()
        {
            _index = (_index - 1 + _abnormalStatusList.Count) % _abnormalStatusList.Count;
            UpdateTemplate();
        }

        private void NextButton()
        {
            _index = (_index + 1) % _abnormalStatusList.Count;

            UpdateTemplate();
        }

        private void ApplyStatus()
        {
            _unit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_template, 3f);
        }
    }
}