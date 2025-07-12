using FrameWork.Tooltip;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    [RequireComponent(typeof(TooltipTrigger))]
    public class UISynergyItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            SynergyItem,
            Icon,
        }
        enum Texts
        {
            Name,
        }
        #endregion

        private TextMeshProUGUI _name;
        private Image _synergyItem;
        private Image _icon;

        private TooltipTrigger _trigger;

        protected override void Initialize()
        {
            _trigger = GetComponent<TooltipTrigger>();

            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _synergyItem = GetImage((int)Images.SynergyItem);
            _icon = GetImage((int)Images.Icon);
            _name = GetText((int)Texts.Name);
        }

        internal void Show(SynergyTemplate template, List<AgentTemplate> allUnits, HashSet<AgentBattleData> activeUnits)
        {
            _name.text = template.displayName;
            _icon.sprite = template.icon;

            var activeTemplates = new HashSet<AgentTemplate>(activeUnits.Select(data => data.agentTemplate));
            int activeCount = allUnits.Count(unit => activeTemplates.Contains(unit));

            foreach (var data in template.synergyDatas)
            {
                if (data.count == activeCount)
                {
                    
                }
            }

            _trigger.SetAction(() =>
            {
                if (_trigger.GetTooltipStyle() is SynergyTooltipStyle synergyTooltip)
                {
                    synergyTooltip.CustomData(template, allUnits, activeUnits);
                }
            });

            base.Show(true);
        }
    }
}