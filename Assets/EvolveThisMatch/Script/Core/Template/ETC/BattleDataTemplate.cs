using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Data/BattleData", fileName = "BattleData", order = 0)]
    public class BattleDataTemplate : ScriptableObject
    {
        [SerializeField, Label("이름")] private string _displayName;

        [Header("기본 설정")]
        [SerializeField, Label("시작 코인")] private int _startCoin;
        [SerializeField, Label("시작 재능의 파편")] private int _startCrystal;
        [SerializeField, Label("패배 조건 유닛 수")] private int _defeatCondition;

        [Header("웨이브")]
        [SerializeField] private WaveLibraryTemplate _waveLibrary;

        [Header("보상")]
        [SerializeField] private List<RewardData> _rewardsPerWave;

        #region 프로퍼티
        public string displayName => _displayName;

        public int startCoin => _startCoin;
        public int startCrystal => _startCrystal;
        public int defeatCondition => _defeatCondition;

        public WaveLibraryTemplate waveLibrary => _waveLibrary;

        public IReadOnlyList<RewardData> rewardsPerWave => _rewardsPerWave;
        #endregion
    }

    [Serializable]
    public class RewardData
    {
        public CurrencyType type;
        public int amount;
    }
}