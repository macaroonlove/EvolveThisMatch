namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_MagicResistance : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.MagicResistance);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.MagicResistance);
        }

        protected override string GetDiscription()
        {
            return "마법 공격으로부터 받는 피해를 감소시킵니다.";
        }
    }
}