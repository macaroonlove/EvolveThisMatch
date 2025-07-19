namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_AttackRange : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.AttackRange);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.AttackRange);
        }

        protected override string GetDiscription()
        {
            return "기본 공격에 적용되는 사거리입니다.\n근거리 유닛은 원형 범위로,\n원거리 유닛은 성벽에서 떨어진 거리 기준으로 적용됩니다.";
        }
    }
}