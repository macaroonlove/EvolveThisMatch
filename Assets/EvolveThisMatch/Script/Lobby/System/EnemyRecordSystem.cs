using CodeStage.AntiCheat.ObscuredTypes;
using EvolveThisMatch.Core;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class EnemyRecordSystem : MonoBehaviour, IBattleSystem
    {
        private Dictionary<int, ObscuredInt> _records = new Dictionary<int, ObscuredInt>();

        public void Initialize()
        {
            // 이전에 강제종료 등으로 인해 보상 획득을 못했을 경우를 대비하여 우선 로드
            LoadRecords();
            // 방치 보상 획득 및 잔여 보상 확인 후 획득 (서버로 보내기)

            // 기록 시작
            BattleManager.Instance.GetSubSystem<EnemySystem>().onDeath += AddKill;
        }

        public void Deinitialize()
        {
            // 기록 종료
            BattleManager.Instance.GetSubSystem<EnemySystem>().onDeath -= AddKill;
        }

        public void AddKill(int enemyId)
        {
            // 서버용 기록 추가
            if (!_records.ContainsKey(enemyId))
                _records[enemyId] = 0;
            _records[enemyId]++;

            // 50마리 죽일 때 마다 PlayerPrefs에 저장
            if (_records[enemyId] % 50 == 0)
            {
                SaveRecords();
            }
        }

        #region Records
        /// <summary>
        /// 서버 전송을 위해 기록을 json으로 변환
        /// </summary>
        public string GetRecords()
        {
            var records = new Dictionary<int, int>();

            foreach (var record in _records)
                records[record.Key] = record.Value;

            return JsonUtility.ToJson(new Serialization<int>(records));
        }

        /// <summary>
        /// 기록 정리하기
        /// </summary>
        public void ClearRecords()
        {
            _records.Clear();
            PlayerPrefs.DeleteKey("EnemyRecord");
        }

        /// <summary>
        /// PlayerPrefs에서 기록 로드하기
        /// </summary>
        private void LoadRecords()
        {
            if (PlayerPrefs.HasKey("EnemyRecord"))
            {
                string json = PlayerPrefs.GetString("EnemyRecord");
                _records = JsonUtility.FromJson<Serialization<ObscuredInt>>(json).ToDictionary();
            }
        }

        /// <summary>
        /// PlayerPrefs에 기록 저장하기
        /// </summary>
        private void SaveRecords()
        {
            string json = JsonUtility.ToJson(new Serialization<ObscuredInt>(_records));
            PlayerPrefs.SetString("EnemyRecord", json);
            PlayerPrefs.Save();
        }
        #endregion
    }

    [System.Serializable]
    public class Serialization<T>
    {
        public List<int> keys;
        public List<T> values;

        // 서버용 저장 방식
        public Serialization(Dictionary<int, T> dict)
        {
            keys = new List<int>(dict.Keys);
            values = new List<T>(dict.Values);
        }

        // 로컬용 저장 방식
        public Dictionary<int, T> ToDictionary()
        {
            var dict = new Dictionary<int, T>();
            for (int i = 0; i < keys.Count; i++)
                dict[keys[i]] = values[i];
            return dict;
        }
    }
}