using EvolveThisMatch.Core;
using FrameWork.UIBinding;

namespace EvolveThisMatch.Battle
{
    /// <summary>
    /// 전투에서 사용되는 스킬 캔버스
    /// </summary>
    public class UISkillView : UIBase
    {
        private UISkillSlot[] _skillSlots;

        protected override void Initialize()
        {
            _skillSlots = GetComponentsInChildren<UISkillSlot>();
        }

        internal void Show(AgentUnit unit)
        {
            unit.agentData.onSyncIncrease += OnSyncChanged;

            OnSyncChanged(unit.agentData);
        }

        internal void Hide(AgentUnit unit)
        {
            unit.agentData.onSyncIncrease -= OnSyncChanged;
        }

        private void OnSyncChanged(AgentBattleData agentData)
        {
            int cnt = agentData.agentTemplate.skillTemplates.Count;

            for (int i = 0; i < _skillSlots.Length; i++)
            {
                if (i < cnt)
                {
                    bool isUnlock = agentData.skillUnlock < i + 1;
                    _skillSlots[i].ShowSkillSlot(agentData.agentUnit, agentData.agentTemplate.skillTemplates[i], isUnlock);
                }
                else
                {
                    _skillSlots[i].Hide(true);
                }
            }
        }
    }
}