using System.Collections;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class WaveSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] protected WaveLibraryTemplate _waveLibrary;

        protected EnemySpawnSystem _enemySpawnSystem;
        protected TimeSystem _timeSystem;

        public WaveLibraryTemplate waveLibrary => _waveLibrary;
        public int currentWaveIndex { get; protected set; }
        public bool isWaveEnd { get; protected set; }
        public bool isSpawnEnd { get; protected set; }
        public Transform spawnPoint { get; protected set; }
        public Transform boundaryPoint { get; protected set; }

        public void Initialize()
        {
            _enemySpawnSystem = BattleManager.Instance.GetSubSystem<EnemySpawnSystem>();
            _timeSystem = BattleManager.Instance.GetSubSystem<TimeSystem>();

            spawnPoint = transform.GetChild(0);
            boundaryPoint = transform.GetChild(1);

            currentWaveIndex = 0;
            isWaveEnd = false;
            isSpawnEnd = false;

            StartCoroutine(CoUpdateWave());
        }

        public void Deinitialize()
        {
            if (this != null)
            {
                StopAllCoroutines();
            }

            _enemySpawnSystem = null;
            _timeSystem = null;

            isWaveEnd = true;
        }

        public void ForceEndWave()
        {
            Deinitialize();
        }

        protected abstract IEnumerator CoUpdateWave();

        protected IEnumerator SpawnWave(WaveTemplate wave, bool isIdle)
        {
            isSpawnEnd = false;

            foreach (var waveInfo in wave.waveInfo)
            {
                // 딜레이 후 적 스폰
                yield return new WaitForSeconds(waveInfo.delayTime);

                var wfs = new WaitForSeconds(waveInfo.spawnInterval);

                for (int i = 0; i < waveInfo.spawnCount; i++)
                {
                    Vector3 spawnPos = spawnPoint.position;
                    spawnPos.y = Random.Range(-4f, 4f);

                    var enemyData = wave.GetEnemyData(waveInfo.rarity);

                    _enemySpawnSystem.SpawnUnit(enemyData, spawnPos, isIdle);

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