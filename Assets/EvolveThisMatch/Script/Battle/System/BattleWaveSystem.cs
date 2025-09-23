using EvolveThisMatch.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Battle
{
    public class BattleWaveSystem : WaveSystem
    {
        private int _currentChapterIndex;

        public event UnityAction<int, float> onWaveChanged;

        protected override IEnumerator CoUpdateWave()
        {
            _currentChapterIndex = 0;
            int totalWaveIndex = 1;
            while (!isWaveEnd)
            {
                if (_timeSystem == null) yield break;

                if (_currentChapterIndex >= _waveLibrary.waves.Count)
                {
                    isWaveEnd = true;
                    yield break;
                }

                var chapter = _waveLibrary.waves[_currentChapterIndex];

                // 챕터 종료 시, 다음 챕터로 이동
                if (currentWaveIndex >= chapter.waves.Count)
                {
                    _currentChapterIndex++;
                    currentWaveIndex = 0;
                    continue;
                }

                var currentWave = chapter.waves[currentWaveIndex];

                // 웨이브 시작
                StartCoroutine(SpawnWave(currentWave, false));

                // 현재 웨이브 시간 알림
                onWaveChanged?.Invoke(totalWaveIndex, currentWave.waveTime);

                // 웨이브 유지 시간만큼 대기
                yield return new WaitForSeconds(currentWave.waveTime);

                currentWaveIndex++;
                totalWaveIndex++;
            }
        }
    }
}