using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 한 웨이브(스테이지)를 관리
    /// </summary>
    [CreateAssetMenu(menuName = "Templates/Unit/Wave", fileName = "Wave", order = 2)]
    public class WaveTemplate : ScriptableObject, IDataWindowEntry
    {
        [SerializeField, Label("식별번호")] private int _id;
        [SerializeField, Label("웨이브 이름")] private string _displayName;

        [Space(10)]
        [SerializeField, Label("웨이브 유지 시간")] private int _waveTime;

        [Tab("미니언")]
        [SerializeField] private EnemyData _minion = new EnemyData();

        [Tab("일반")]
        [SerializeField] private EnemyData _common = new EnemyData();

        [Tab("엘리트")]
        [SerializeField] private EnemyData _elite = new EnemyData();

        [Tab("보스")]
        [SerializeField] private EnemyData _boss = new EnemyData();
        [EndTab]

        [SerializeField] private List<WaveInfo> _waveInfo = new List<WaveInfo>();

        #region 프로퍼티
        public int id => _id;
        public Sprite sprite => null;
        public string displayName => _displayName;
        public int waveTime => _waveTime;

        public IReadOnlyList<WaveInfo> waveInfo => _waveInfo;
        #endregion

        #region 값 변경 메서드
        internal void SetId(int id) => _id = id;
        public void SetDisplayName(string name) => _displayName = name;
        public void SetWaveTime(int waveTime) => _waveTime = waveTime;
        #endregion

        public EnemyData GetEnemyData(EEnemyRarity rarity)
        {
            switch (rarity)
            {
                case EEnemyRarity.Minion:
                    return _minion;
                case EEnemyRarity.Common:
                    return _common;
                case EEnemyRarity.Elite:
                    return _elite;
                case EEnemyRarity.Boss:
                    return _boss;
            }

            return null;
        }
    }

    [Serializable]
    public class EnemyData
    {
        [SerializeField, Label("유닛")] private EnemyTemplate _template;
        [SerializeField, Label("HP")] private int _hp;
        [SerializeField, Label("ATK")] private int _atk;

        [Header("인게임 재화")]
        [SerializeField, Label("코인")] private int _coin;
        [SerializeField, Label("크리스탈")] private int _crystal;
        
        [Header("방치형 재화")]
        [SerializeField, Label("골드")] private int _gold;
        [SerializeField, Label("전리품")] private int _loot;

        #region 프로퍼티
        public EnemyTemplate template => _template;
        public int hp => _hp;
        public int atk => _atk;
        public int coin => _coin;
        public int crystal => _crystal;
        public int gold => _gold;
        public int loot => _loot;
        #endregion

        #region 값 변경 메서드
        internal void SetTemplate(EnemyTemplate template) => _template = template;
        internal void SetHP(int hp) => _hp = hp;
        internal void SetATK(int atk) => _atk = atk;
        internal void SetCoin(int coin) => _coin = coin;
        internal void SetCrystal(int crystal) => _crystal = crystal;
        internal void SetGold(int gold) => _gold = gold;
        internal void SetLoot(int loot) => _loot = loot;
        #endregion
    }

    [Serializable]
    public class WaveInfo
    {
        [SerializeField, Label("유닛 타입")] private EEnemyRarity _rarity;

        [Tooltip("웨이브 시작 시간을 기준으로 몇 초 있다가 스폰할 것인지")]
        [SerializeField, Label("딜레이")] private float _delayTime;
        [SerializeField, Label("스폰할 유닛의 수")] private int _spawnCount = 1;
        [SerializeField, Label("스폰 간격")] private float _spawnInterval = 0;

        #region 프로퍼티
        public EEnemyRarity rarity => _rarity;
        public float delayTime => _delayTime;
        public int spawnCount => _spawnCount;
        public float spawnInterval => _spawnInterval;
        #endregion
    }
}
