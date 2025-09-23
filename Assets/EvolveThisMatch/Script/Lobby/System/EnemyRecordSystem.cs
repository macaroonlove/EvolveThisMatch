using CodeStage.AntiCheat.ObscuredTypes;
using EvolveThisMatch.Core;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class EnemyRecordSystem : MonoBehaviour, IBattleSystem
    {
        private Dictionary<string, ObscuredInt> _serverRecords = new Dictionary<string, ObscuredInt>();
        private Dictionary<string, ObscuredInt> _localRecords = new Dictionary<string, ObscuredInt>();

        public void Initialize()
        {
            LoadLocalRecords();
        }

        public void Deinitialize()
        {
            
        }

        public void AddKill(string enemyId)
        {
            // 서버용 기록 추가
            if (!_serverRecords.ContainsKey(enemyId))
                _serverRecords[enemyId] = 0;
            _serverRecords[enemyId]++;

            // 로컬용 기록 추가
            if (!_localRecords.ContainsKey(enemyId))
                _localRecords[enemyId] = 0;
            _localRecords[enemyId]++;

            // 일정 횟수마다 PlayerPrefs 저장
            if (_serverRecords[enemyId] % 50 == 0)
            {
                SaveLocalRecords();
            }
        }

        #region Server
        /// <summary>
        /// 서버용 기록 불러오기
        /// </summary>
        public string GetServerRecords()
        {
            var records = new Dictionary<string, int>();

            foreach (var record in _serverRecords)
                records[record.Key] = record.Value;

            return JsonUtility.ToJson(new Serialization<int>(records));
        }

        public void LoadLocalRecords()
        {
            if (PlayerPrefs.HasKey("EnemyRecord"))
            {
                string json = PlayerPrefs.GetString("EnemyRecord");
                _serverRecords = JsonUtility.FromJson<Serialization<ObscuredInt>>(json).ToDictionary();
            }
        }

        public void SaveLocalRecords()
        {
            string json = JsonUtility.ToJson(new Serialization<ObscuredInt>(_serverRecords));
            PlayerPrefs.SetString("EnemyRecord", json);
            PlayerPrefs.Save();
        }

        public void ClearServerRecords()
        {
            _serverRecords.Clear();
            PlayerPrefs.DeleteKey("EnemyRecord");
        }
        #endregion

        public void ClearLocalRecords()
        {
            _localRecords.Clear();
        }
    }

    [System.Serializable]
    public class Serialization<T>
    {
        public List<string> keys;
        public List<T> values;

        // 서버용 저장 방식
        public Serialization(Dictionary<string, T> dict)
        {
            keys = new List<string>(dict.Keys);
            values = new List<T>(dict.Values);
        }

        // 로컬용 저장 방식
        public Dictionary<string, T> ToDictionary()
        {
            var dict = new Dictionary<string, T>();
            for (int i = 0; i < keys.Count; i++)
                dict[keys[i]] = values[i];
            return dict;
        }
    }
}