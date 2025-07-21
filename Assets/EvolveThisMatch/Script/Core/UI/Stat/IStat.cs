namespace EvolveThisMatch.Core
{
    public interface IBattleStat 
    {
        void Initialize(Unit unit);
        void Deinitialize();
    }
    
    public interface IGeneralStat
    {
        void Initialize(AgentTemplate template);
        void Initialize(EnemyTemplate template);
        void Clear();
        void Deinitialize();
    }
}