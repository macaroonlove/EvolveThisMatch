using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using FrameWork.Tooltip;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class SynergyTooltipStyle : TooltipStyle
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Icon,
        }
        enum Texts
        {
            DisplayName,
            Description,
        }
        #endregion

        private Image _icon;
        private TextMeshProUGUI _displayNameText;
        private TextMeshProUGUI _descriptionText;

        private UISynergyUnit[] _units;
        private StringBuilder _description = new StringBuilder(128);
        private bool isInitialize;

#if UNITY_EDITOR
        public override TooltipData CreateField()
        {
            var data = new TooltipData();

            return data;
        }
#endif

        public override void ApplyData(TooltipData data)
        {
            data.action?.Invoke();
        }

        protected override void Initialize()
        {
            if (isInitialize) return;

            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _icon = GetImage((int)Images.Icon);
            _displayNameText = GetText((int)Texts.DisplayName);
            _descriptionText = GetText((int)Texts.Description);

            _units = GetComponentsInChildren<UISynergyUnit>();

            isInitialize = true;
        }

        public async void CustomData(SynergyTemplate template, List<AgentTemplate> allUnits, HashSet<AgentBattleData> activeUnits)
        {
            Initialize();

            _icon.sprite = template.icon;
            _displayNameText.text = template.displayName;

            var activeTemplates = new HashSet<AgentTemplate>(activeUnits.Select(data => data.agentTemplate));
            int activeCount = allUnits.Count(unit => activeTemplates.Contains(unit));

            _description.Clear();
            _description.Append($"{template.description}");

            foreach (var effect in template.synergyDatas)
            {
                if (effect.count != activeCount)
                {
                    _description.Append($"\n<color=#888888>({effect.count}) {effect.buff.description}</color>");
                }
                else
                {
                    _description.Append($"\n({effect.count}) {effect.buff.description}");
                }
            }

            _descriptionText.text = _description.ToString();

            for (int i = 0; i < _units.Length; i++)
            {
                if (i < allUnits.Count)
                {
                    var agentTemplate = allUnits[i];
                    bool isActive = activeUnits.Any(data => data.agentTemplate == agentTemplate);
                    _units[i].Show(agentTemplate, isActive);
                }
                else
                {
                    _units[i].Hide(true);
                }
            }

            await UniTask.Yield();

            var newHeight = _descriptionText.textInfo.lineCount * 30;
            Vector2 sizeDelta = _descriptionText.rectTransform.sizeDelta;
            sizeDelta.y = newHeight;
            _descriptionText.rectTransform.sizeDelta = sizeDelta;
        }
    }
}