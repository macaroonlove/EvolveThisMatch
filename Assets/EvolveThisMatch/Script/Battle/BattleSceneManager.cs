using EvolveThisMatch.Core;
using FrameWork;

namespace EvolveThisMatch.Battle
{
    public class BattleSceneManager : Singleton<BattleSceneManager>
    {
        private void Start()
        {
            // 전투 시작
            BattleManager.instance.InitializeBattle();
        }
    }
}