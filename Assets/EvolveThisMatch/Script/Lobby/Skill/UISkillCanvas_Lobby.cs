using EvolveThisMatch.Core;
using FrameWork.UIBinding;

namespace EvolveThisMatch.Lobby
{
    public class UISkillCanvas_Lobby : UISkillCanvas
    {
        private UISkillSlot_Lobby[] _skillSlots;

        protected override void Initialize()
        {
            _skillSlots = GetComponentsInChildren<UISkillSlot_Lobby>();
        }

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