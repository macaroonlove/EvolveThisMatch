using EvolveThisMatch.Save;

namespace EvolveThisMatch.Core
{
    public class UIGeneralStat_ATK : UIGeneralStat, IGeneralStat
    {
        private GlobalStatusSystem _globalStatusSystem;
        private EffectContext _effectContext;

        protected override void Awake()
        {
            base.Awake();

            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();
            _effectContext = new EffectContext();
        }

        public void Initialize(AgentTemplate template, AgentSaveData.Agent owned)
        {
            float result = template.ATK;

            _effectContext.agentSaveData = owned;

            foreach (var instance in _globalStatusSystem.ATKMultiplierDataEffects)
            {
                result *= (1 + instance.effect.GetValue(_effectContext, instance.context));
            }

            Apply(result);
        }

        public bool IsAvailable(AgentTemplate template)
        {
            return IsIncludeJob(template);
        }

        protected override string GetDiscription()
        {
            return "적에게 주는 피해량을 결정하는 기본적인 전투력입니다.";
        }
    }
}