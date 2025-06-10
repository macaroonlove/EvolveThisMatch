using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ActiveSkillTestCanvas : MonoBehaviour
    {
        internal void Initialize(Unit unit)
        {
            var agent = unit as AgentUnit;

            var buttons = GetComponentsInChildren<UIActiveSkillExecuteButton>();

            List<ActiveSkillTemplate> templates = new List<ActiveSkillTemplate>();

            if (unit is AgentUnit agentUnit)
            {
                if (agentUnit.template.skillTreeGraph == null)
                {
                    foreach (var button in buttons)
                    {
                        button.Hide(true);
                    }
                    return;
                }
                templates = InitializeActiveSkill(agentUnit.template.skillTreeGraph);
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                if (enemyUnit.template.skillTreeGraph == null)
                {
                    foreach (var button in buttons)
                    {
                        button.Hide(true);
                    }
                    return;
                }
                templates = InitializeActiveSkill(enemyUnit.template.skillTreeGraph);
            }

            var minIndex = Mathf.Min(templates.Count, buttons.Length);

            for (int index = 0; index < 4; index++)
            {
                if (minIndex > index)
                {
                    buttons[index].Show(agent, templates[index]);
                    buttons[index].Show(true);
                }
                else
                {
                    buttons[index].Hide(true);
                }
            }
        }

        private List<ActiveSkillTemplate> InitializeActiveSkill(SkillTreeGraph skillTree)
        {
            List<ActiveSkillTemplate> results = new List<ActiveSkillTemplate>();

            foreach (var node in skillTree.nodes)
            {
                if (node is SkillNode skillNode && skillNode.skillTemplate is ActiveSkillTemplate skill)
                {
                    results.Add(skill);
                }
            }

            return results;
        }
    }
}