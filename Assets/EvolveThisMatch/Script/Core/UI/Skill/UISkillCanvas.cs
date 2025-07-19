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

        /// <summary>
        /// 전투에서 사용
        /// </summary>
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

        /// <summary>
        /// 전투를 제외한 상황에서 사용
        /// </summary>
        public void ShowSkill(AgentTemplate template)
        {
            int cnt = template.skillTemplates.Count;

            for (int i = 0; i < _skillSlots.Length; i++)
            {
                if (i < cnt)
                {
                    _skillSlots[i].ShowSkillSlot(template.skillTemplates[i]);
                }
                else
                {
                    _skillSlots[i].Hide(true);
                }
            }
        }
    }
}