using EvolveThisMatch.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class LobbyWaveSystem : WaveSystem
    {
        private WaveTemplate _currentWave;

        public WaveTemplate currentWave => _currentWave;

        public event UnityAction onChangeWave;

        protected override IEnumerator CoUpdateWave()
        {
            if (_currentWave == null)
            {
                yield break;
            }

            while (true)
            {
                if (_timeSystem == null) yield break;

                // 웨이브 시작
                StartCoroutine(SpawnWave(_currentWave, true));

                // 웨이브 유지 시간만큼 대기
                yield return new WaitForSeconds(_currentWave.waveTime);
            }
        }

        public void ChangeWave(WaveTemplate template)
        {
            // 전투 중단
            BattleManager.Instance.DeinitializeBattle();

            // 웨이브 변경
            _currentWave = template;

            onChangeWave?.Invoke();

            // 전투 다시 시작
            BattleManager.Instance.InitializeBattle();
        }

        public void StopWave()
        {
            isWaveEnd = true;
        }
    }
}