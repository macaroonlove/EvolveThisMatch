using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class WaveSystem : MonoBehaviour, IBattleSystem
    {
        private EnemySpawnSystem _enemySpawnSystem;
        private TimeSystem _timeSystem;
        private WaveLibraryTemplate _waveLibrary;

        private Transform _spawnPoint;
        private int _currentWaveIndex;

        internal bool isWaveEnd { get; private set; }
        internal bool isSpawnEnd { get; private set; }
        internal Transform boundaryPoint { get; private set; }

        public event UnityAction<int, float> onWaveChanged;

        public void Initialize()
        {
            _enemySpawnSystem = BattleManager.Instance.GetSubSystem<EnemySpawnSystem>();
            _timeSystem = BattleManager.Instance.GetSubSystem<TimeSystem>();
            _waveLibrary = GameDataManager.Instance.waveLibrary;
            _spawnPoint = transform.GetChild(0);
            boundaryPoint = transform.GetChild(1);

            isWaveEnd = false;
        }

        public void Deinitialize()
        {
            _enemySpawnSystem = null;
            _timeSystem = null;
            isWaveEnd = true;

            StopAllCoroutines();
        }

        internal void ForceEndWave()
        {
            Deinitialize();
        }

        private void Update()
        {
            if (_timeSystem == null) return;
            if (isWaveEnd) return;

            if (_currentWaveIndex >= _waveLibrary.waves.Count)
            {
                isWaveEnd = true;
                return;
            }

            if (_timeSystem.currentTime >= _waveLibrary.waves[_currentWaveIndex].spawnTime)
            {
                WaveTemplate currentWave = _waveLibrary.waves[_currentWaveIndex];
                StartCoroutine(SpawnWave(currentWave));

                onWaveChanged?.Invoke(_currentWaveIndex + 1, (_waveLibrary.waves.Count == _currentWaveIndex + 1) ? 0 : _waveLibrary.waves[_currentWaveIndex + 1].spawnTime - _waveLibrary.waves[_currentWaveIndex].spawnTime);

                _currentWaveIndex++;
            }
        }

        private IEnumerator SpawnWave(WaveTemplate wave)
        {
            isSpawnEnd = false;

            foreach (var waveInfo in wave.waveInfo)
            {
                // 딜레이 후 적 스폰
                yield return new WaitForSeconds(waveInfo.delayTime);

                var wfs = new WaitForSeconds(waveInfo.spawnInterval);

                for (int i = 0; i < waveInfo.spawnCount; i++)
                {
                    Vector3 spawnPos = _spawnPoint.position;
                    spawnPos.y = Random.Range(-4f, 4f);

                    _enemySpawnSystem.SpawnUnit(waveInfo.template, spawnPos, waveInfo.coin, waveInfo.crystal);

                    if (i < waveInfo.spawnCount - 1)
                    {
                        // 스폰 간격 대기
                        yield return wfs;
                    }
                }
            }

            isSpawnEnd = true;
        }
    }
}