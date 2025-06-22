using FrameWork.UIBinding;

namespace EvolveThisMatch.Core
{
    public class UISkillCanvas : UIBase
    {
        UISkillSlot[] _skillSlots;

        protected override void Initialize()
        {
            _skillSlots = GetComponentsInChildren<UISkillSlot>();
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