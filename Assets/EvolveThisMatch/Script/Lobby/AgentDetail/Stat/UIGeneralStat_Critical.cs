using EvolveThisMatch.Save;

namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_Critical : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template, AgentSaveData.Agent owned)
        {
            Apply(template.CriticalHitChance);
        }

        public bool IsAvailable(AgentTemplate template)
        {
            return IsIncludeJob(template);
        }

        protected override string GetDiscription()
        {
            return "기본 공격이 치명타로 적중할 확률입니다.\n치명타가 발생하면 최종 피해량이 2배로 증가합니다.";
        }
    }
}