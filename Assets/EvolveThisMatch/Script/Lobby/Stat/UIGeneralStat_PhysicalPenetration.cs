namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_PhysicalPenetration : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.PhysicalPenetration);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.PhysicalPenetration);
        }

        protected override string GetDiscription()
        {
            return "방어력을 무시하여 피해를 증가시킵니다.\n물리 관통력이 방어력을 초과하면 추가 피해가 발생합니다.";
        }
    }
}