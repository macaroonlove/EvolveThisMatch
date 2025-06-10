namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_HP : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.ATK);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.ATK);
        }

        protected override string GetDiscription()
        {
            return "받는 피해량만큼 감소되는 체력입니다.";
        }
    }
}