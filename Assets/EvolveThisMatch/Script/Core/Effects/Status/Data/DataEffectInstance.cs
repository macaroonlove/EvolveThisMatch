namespace EvolveThisMatch.Core
{
    public readonly struct DataEffectInstance<T> where T : DataEffectBase
    {
        public readonly T effect;
        public readonly EffectContext context;

        public DataEffectInstance(T effect, EffectContext context)
        {
            this.effect = effect;
            this.context = context;
        }
    }

    /// <summary>
    /// 해당 Effect가 어떠한 요소에 의해 발생한 것인지 표시할 수 있도록 확장한 Instance
    /// </summary>
    public readonly struct StatDataEffectInstance<T> where T : DataEffectBase
    {
        public readonly T effect;
        public readonly EffectContext context;
        public readonly string displayName;

        public StatDataEffectInstance(T effect, EffectContext context, string displayName)
        {
            this.effect = effect;
            this.context = context;
            this.displayName = displayName;
        }
    }
}