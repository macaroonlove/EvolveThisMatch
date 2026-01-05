using EvolveThisMatch.Save;

namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_AttackTerm : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template, AgentSaveData.Agent owned)
        {
            Apply(template.AttackTerm);
        }

        public bool IsAvailable(AgentTemplate template)
        {
            return IsIncludeJob(template);
        }

        protected override string GetDiscription()
        {
            return "기본 공격이 나가는 시간 간격입니다.";
        }
    }
}