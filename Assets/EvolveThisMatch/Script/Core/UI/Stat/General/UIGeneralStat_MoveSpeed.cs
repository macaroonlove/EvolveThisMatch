namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_MoveSpeed : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.MoveSpeed);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.MoveSpeed);
        }

        protected override string GetDiscription()
        {
            return "유닛이 전장 위를 이동하는 속도입니다.";
        }
    }
}