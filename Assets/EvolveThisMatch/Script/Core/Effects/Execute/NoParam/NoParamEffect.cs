namespace EvolveThisMatch.Core
{
    /// <summary>
    /// Execute에 매개변수를 사용하지 않는 Effect
    /// </summary>
    public abstract class NoParamEffect : ExecuteEffect
    {
        public abstract void Execute(EffectContext effectContext);
    }
}