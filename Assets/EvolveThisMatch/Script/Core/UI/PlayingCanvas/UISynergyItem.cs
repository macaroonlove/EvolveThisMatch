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

            for (int i = 0; i < template.synergyDatas.Count; i++)
            {
                var data = template.synergyDatas[i];
                var units = GetTarget(data, activeUnits);

                foreach (var unit in units)
                {
                    var buffAbility = unit.GetAbility<BuffAbility>();

                    if (data.count == activeCount)
                    {
                        buffAbility.ApplyBuff(data.buff, int.MaxValue);
                    }
                    else
                    {
                        buffAbility.RemoveBuff(data.buff);
                    }
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

        private List<Unit> GetTarget(SynergyData data, HashSet<AgentBattleData> activeUnits)
        {
            List<Unit> units = new List<Unit>();

            if (data.isSynergyUnit)
            {
                foreach (var unitData in activeUnits)
                {
                    units.Add(unitData.agentUnit);
                }
            }
            else
            {
                if ((data.unitType & (EUnitType.Agent | EUnitType.Summon)) != 0)
                {
                    units.AddRange(BattleManager.Instance.GetSubSystem<AllySystem>().GetAllAllies(data.unitType));
                }

                if ((data.unitType & EUnitType.Enemy) != 0)
                {
                    units.AddRange(BattleManager.Instance.GetSubSystem<EnemySystem>().GetAllEnemies());
                }
            }

            return units;
        }
    }
}