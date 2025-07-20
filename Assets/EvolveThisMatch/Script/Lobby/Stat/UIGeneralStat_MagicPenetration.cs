namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_MagicPenetration : UIGeneralStat, IGeneralStat
    {
        public void Initialize(AgentTemplate template)
        {
            Apply(template.MagicPenetration);
        }

        public void Initialize(EnemyTemplate template)
        {
            Apply(template.MagicPenetration);
        }

        protected override string GetDiscription()
        {
            return "마법 저항력을 무시하여 피해를 증가시킵니다.\n마법 관통력이 마법 저항력을 초과하면 추가 피해가 발생합니다.";
        }
    }
}