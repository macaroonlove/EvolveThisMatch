using EvolveThisMatch.Core;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Battle
{
    /// <summary>
    /// 전투 패배를 판단하는 시스템
    /// </summary>
    public class DefeatSystem : MonoBehaviour, IBattleSystem
    {
        private EnemySystem _enemySystem;
        private WaveSystem _waveSystem;

        internal event UnityAction onDefeat;

        public void Initialize()
        {
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();
            _waveSystem = BattleManager.Instance.GetSubSystem<WaveSystem>();

            _enemySystem.onRegist += OnRegist;
        }

        public void Deinitialize()
        {
            _enemySystem.onRegist -= OnRegist;
        }

        private void OnRegist(Unit unit)
        {
            if (_enemySystem.enemyCount > 100)
            {
                Defeat();
            }
        }

        private void Defeat()
        {
            _waveSystem.ForceEndWave();
            //BattleManager.Instance.DeinitializeBattle();
            onDefeat?.Invoke();
        }
    }
}