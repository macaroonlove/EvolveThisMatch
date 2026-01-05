using EvolveThisMatch.Save;

namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_MoveSpeed : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template, AgentSaveData.Agent owned)
        {
            Apply(template.MoveSpeed);
        }

        public bool IsAvailable(AgentTemplate template)
        {
            return IsIncludeJob(template);
        }

        protected override string GetDiscription()
        {
            return "유닛이 전장 위를 이동하는 속도입니다.";
        }
    }
}