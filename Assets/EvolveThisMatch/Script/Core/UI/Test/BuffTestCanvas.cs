using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BuffTestCanvas : UIBase
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

        [SerializeField] private List<BuffTemplate> _buffList = new List<BuffTemplate>();

        private Unit _unit;
        private BuffTemplate _template;
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
            _template = _buffList[_index];
            _statusNameText.text = _template.displayName;
        }

        private void PrevButton()
        {
            _index = (_index - 1 + _buffList.Count) % _buffList.Count;
            UpdateTemplate();
        }

        private void NextButton()
        {
            _index = (_index + 1) % _buffList.Count;

            UpdateTemplate();
        }

        private void ApplyStatus()
        {
            _unit.GetAbility<BuffAbility>().ApplyBuff(_template, 3f);
        }
    }
}