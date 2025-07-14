using FrameWork.Editor;
using ScriptableObjectArchitecture;
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

        [Header("보상")]
        [SerializeField, Label("골드")] private ObscuredIntVariable _goldVariable;
        [SerializeField, Label("재능의 정수")] private ObscuredIntVariable _essenceVariable;
        [SerializeField, Label("전리품")] private ObscuredIntVariable _lootVariable;

        [SerializeField, Label("웨이브 당 골드 획득량")] private int _goldPerWave;
        [SerializeField, Label("웨이브 당 재능의 정수 획득량")] private int _essencePerWave;
        [SerializeField, Label("웨이브 당 전리품 획득량")] private int _lootPerWave;

        [Header("웨이브")]
        [SerializeField] private WaveLibraryTemplate _waveLibrary;

        #region 프로퍼티
        public string displayName => _displayName;

        public int startCoin => _startCoin;
        public int startCrystal => _startCrystal;
        public int defeatCondition => _defeatCondition;

        public ObscuredIntVariable goldVariable => _goldVariable;
        public ObscuredIntVariable essenceVariable => _essenceVariable;
        public ObscuredIntVariable lootVariable => _lootVariable;

        public int goldPerWave => _goldPerWave;
        public int essencePerWave => _essencePerWave;
        public int lootPerWave => _lootPerWave;

        public WaveLibraryTemplate waveLibrary => _waveLibrary;
        #endregion
    }
}