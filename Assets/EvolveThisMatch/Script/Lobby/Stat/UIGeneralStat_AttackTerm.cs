namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_AttackTerm : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.AttackTerm);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.AttackTerm);
        }

        protected override string GetDiscription()
        {
            return "기본 공격이 나가는 시간 간격입니다.";
        }
    }
}