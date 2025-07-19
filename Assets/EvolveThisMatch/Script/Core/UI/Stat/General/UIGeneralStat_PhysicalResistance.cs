namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_PhysicalResistance : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.PhysicalResistance);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.PhysicalResistance);
        }

        protected override string GetDiscription()
        {
            return "물리 공격으로부터 받는 피해를 감소시킵니다.";
        }
    }
}