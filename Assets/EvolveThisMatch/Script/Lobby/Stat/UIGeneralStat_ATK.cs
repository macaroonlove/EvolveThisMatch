namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_ATK : UIGeneralStat, IGeneralStat
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
            return "적에게 주는 피해량을 결정하는 기본적인 공격력입니다.";
        }
    }
}