using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using UnityEngine;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class ProfileSaveData
    {
        public string displayName;

        [Tooltip("튜토리얼 클리어 여부")]
        public bool isClearTutorial;

        [Tooltip("골드")]
        public int gold;

        [Tooltip("세계석")]
        public int essence;

        [Tooltip("전리품")]
        public int loot;

        [Tooltip("행동력")]
        public int action;

        [Tooltip("일반 유닛 책갈피")]
        public int defaultUnitBookmark;

        [Tooltip("픽업 유닛 책갈피")]
        public int pickUpUnitBookmark;

        [Tooltip("찢어진 책 조각")]
        public int bookFragment;

        [Tooltip("픽업 유닛 책갈피")]
        public int unitLimit;

        [Tooltip("말캉 버터")]
        public int butter;
        [Tooltip("갈빗살 꼬치")]
        public int skewers;
        [Tooltip("골수 스튜")]
        public int stew;
        [Tooltip("하트빔 스테이크")]
        public int steak;

        [Tooltip("세계의 흔적")]
        public int powder;
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/ProfileSaveData", fileName = "ProfileSaveData", order = 0)]
    public class ProfileSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private ProfileSaveData _data;

        [Header("기본")]
        [SerializeField] private ObscuredIntVariable _goldVariable;
        [SerializeField] private ObscuredIntVariable _essenceVariable;
        [SerializeField] private ObscuredIntVariable _lootVariable;
        [SerializeField] private ObscuredIntVariable _actionVariable;

        [Header("뽑기")]
        [SerializeField] private ObscuredIntVariable _defaultUnitBookmarkVariable;
        [SerializeField] private ObscuredIntVariable _pickUpUnitBookmarkVariable;
        [SerializeField] private ObscuredIntVariable _bookFragmentVariable;
        [SerializeField] private ObscuredIntVariable _unitLimitVariable;

        [Header("부서_식품")]
        [SerializeField] private ObscuredIntVariable _butterVariable;
        [SerializeField] private ObscuredIntVariable _skewersVariable;
        [SerializeField] private ObscuredIntVariable _stewVariable;
        [SerializeField] private ObscuredIntVariable _steakVariable;

        [Header("부서_가공")]
        [SerializeField] private ObscuredIntVariable _powderVariable;

        public bool isLoaded { get; private set; }

        public string displayName { get => _data.displayName; set => _data.displayName = value; }
        public bool isClearTutorial { get => _data.isClearTutorial; set => _data.isClearTutorial = value; }

        public override void SetDefaultValues()
        {
            _data = new ProfileSaveData();

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<ProfileSaveData>(json);

            if (_data != null)
            {
                isLoaded = true;

                // 기본
                _goldVariable.Value = _data.gold;
                _essenceVariable.Value = _data.essence;
                _lootVariable.Value = _data.loot;
                _actionVariable.Value = _data.action;

                // 뽑기
                _defaultUnitBookmarkVariable.Value = _data.defaultUnitBookmark;
                _pickUpUnitBookmarkVariable.Value = _data.pickUpUnitBookmark;
                _bookFragmentVariable.Value = _data.bookFragment;
                _unitLimitVariable.Value = _data.unitLimit;

                // 식품부
                _butterVariable.Value = _data.butter;
                _skewersVariable.Value = _data.skewers;
                _stewVariable.Value = _data.stew;
                _steakVariable.Value = _data.steak;

                // 가공부
                _powderVariable.Value = _data.powder;
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            // 기본
            _data.gold = _goldVariable.Value;
            _data.essence = _essenceVariable.Value;
            _data.loot = _lootVariable.Value;

            _data.action = _actionVariable.Value;

            // 뽑기
            _data.defaultUnitBookmark = _defaultUnitBookmarkVariable.Value;
            _data.pickUpUnitBookmark = _pickUpUnitBookmarkVariable.Value;
            _data.bookFragment = _bookFragmentVariable.Value;
            _data.unitLimit = _unitLimitVariable.Value;

            // 식품부
            _data.butter = _butterVariable.Value;
            _data.skewers = _skewersVariable.Value;
            _data.stew = _stewVariable.Value;
            _data.steak = _steakVariable.Value;

            // 가공부
            _data.powder = _powderVariable.Value;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }
    }
}