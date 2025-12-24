namespace EvolveThisMatch.Core
{
    public interface IMutableValueBindingProvider
    {
        bool TryGetBindValue(string bindKey, EffectContext context, out string value);
    }
}