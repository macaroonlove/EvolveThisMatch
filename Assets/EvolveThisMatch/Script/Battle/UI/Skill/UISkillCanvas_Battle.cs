using EvolveThisMatch.Core;

namespace EvolveThisMatch.Battle
{
    /// <summary>
    /// 전투에서 사용되는 스킬 캔버스
    /// </summary>
    public class UISkillCanvas_Battle : UISkillCanvas
    {
        private UISkillSlot_Battle[] _skillSlots;

        protected override void Initialize()
        {
            _skillSlots = GetComponentsInChildren<UISkillSlot_Battle>();
        }

        internal void ShowSkill(AgentUnit unit)
        {
            int cnt = unit.template.skillTemplates.Count;

            for (int i = 0; i < _skillSlots.Length; i++)
            {
                if (i < cnt)
                {
                    _skillSlots[i].ShowSkillSlot(unit, unit.template.skillTemplates[i]);
                }
                else
                {
                    _skillSlots[i].Hide(true);
                }
            }
        }
    }
}