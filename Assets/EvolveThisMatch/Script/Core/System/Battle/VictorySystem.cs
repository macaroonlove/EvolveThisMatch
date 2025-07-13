using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 전투 패배를 판단하는 시스템
    /// </summary>
    public class VictorySystem : MonoBehaviour, IBattleSystem
    {
        private EnemySystem _enemySystem;
        private WaveSystem _waveSystem;

        internal event UnityAction onVictory;

        public void Initialize()
        {
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();
            _waveSystem = BattleManager.Instance.GetSubSystem<WaveSystem>();

            _enemySystem.onDeregist += OnChangeEnemyCount;
        }

        public void Deinitialize()
        {
            _enemySystem.onDeregist -= OnChangeEnemyCount;
        }

        private void OnChangeEnemyCount(Unit unit)
        {
            if (_waveSystem.isWaveEnd && _waveSystem.isSpawnEnd && _enemySystem.enemyCount == 0)
            {
                Victory();
            }
        }

        private void Victory()
        {
            //BattleManager.Instance.DeinitializeBattle();
            onVictory?.Invoke();
        }
    }
}