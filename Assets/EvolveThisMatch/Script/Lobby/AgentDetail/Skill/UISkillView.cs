using EvolveThisMatch.Core;
using FrameWork.UIBinding;

namespace EvolveThisMatch.Lobby
{
    public class UISkillView : UIBase
    {
        private UISkillSlot[] _skillSlots;

        protected override void Initialize()
        {
            _skillSlots = GetComponentsInChildren<UISkillSlot>();
        }

        public void Show(AgentTemplate template)
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