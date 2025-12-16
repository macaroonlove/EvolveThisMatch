using EvolveThisMatch.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class LobbyWaveSystem : WaveSystem
    {
        private int _currentCategory;
        private WaveTemplate _currentWave;
        private BlockSystem _blockSystem;

        public int currentCategory => _currentCategory;
        public WaveTemplate currentWave => _currentWave;

        public event UnityAction onChangeWave;

        public override void Initialize()
        {
            base.Initialize();

            _blockSystem = BattleManager.Instance.GetSubSystem<BlockSystem>();
        }

        protected override IEnumerator CoUpdateWave()
        {
            if (_currentWave == null) yield break;

            while (true)
            {
                // 웨이브 시작
                StartCoroutine(SpawnWave(_currentWave));

                // 웨이브 유지 시간만큼 대기
                yield return new WaitForSeconds(_currentWave.waveTime);

                // 100마리 이상 스폰되면 더 이상 스폰되지 않도록 대기
                yield return new WaitUntil(() => _blockSystem == null || _blockSystem.blockCount <= 100);
            }
        }

        public void ChangeWave(int category, WaveTemplate template)
        {
            // 전투 중단
            BattleManager.Instance.DeinitializeBattle();

            // 풀 비우기
            CoreManager.Instance.GetSubSystem<PoolSystem>().Deinitialize();

            // 웨이브 변경
            _currentCategory = category;
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