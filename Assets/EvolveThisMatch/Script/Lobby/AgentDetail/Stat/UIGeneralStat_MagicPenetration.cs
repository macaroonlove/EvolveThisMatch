using EvolveThisMatch.Save;

namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_MagicPenetration : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template, AgentSaveData.Agent owned)
        {
            Apply(template.MagicPenetration);
        }

        public bool IsAvailable(AgentTemplate template)
        {
            return IsIncludeJob(template);
        }

        protected override string GetDiscription()
        {
            return "마법 저항력을 무시하여 피해를 증가시킵니다.\n마법 관통력이 마법 저항력을 초과하면 추가 피해가 발생합니다.";
        }
    }
}