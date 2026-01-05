using EvolveThisMatch.Save;

namespace EvolveThisMatch.Core
{
    public interface IBattleStat 
    {
        void Initialize(Unit unit);
        void Deinitialize();
    }
    
    public interface IGeneralStat
    {
        void Initialize(AgentTemplate template, AgentSaveData.Agent owned);
        void Clear();
        void Deinitialize();
        bool IsAvailable(AgentTemplate template);
    }
}