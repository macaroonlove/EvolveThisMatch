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
        public int Gold;

        [Tooltip("세계석")]
        public int Essence;

        [Tooltip("전리품")]
        public int Loot;

        [Tooltip("행동력")]
        public int Action;

        [Tooltip("일반 유닛 책갈피")]
        public int DefaultUnitBookmark;

        [Tooltip("픽업 유닛 책갈피")]
        public int PickUpUnitBookmark;

        [Tooltip("찢어진 책 조각")]
        public int BookFragments;

        [Tooltip("픽업 유닛 책갈피")]
        public int UnitLimit;

        [Tooltip("말캉 버터")]
        public int SmoothButter;
        [Tooltip("갈빗살 꼬치")]
        public int RibSkewers;
        [Tooltip("골수 스튜")]
        public int BoneStew;
        [Tooltip("하트빔 스테이크")]
        public int HeartSteak;

        [Tooltip("세계의 흔적")]
        public int Powder;
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
                _goldVariable.Value = _data.Gold;
                _essenceVariable.Value = _data.Essence;
                _lootVariable.Value = _data.Loot;
                _actionVariable.Value = _data.Action;

                // 뽑기
                _defaultUnitBookmarkVariable.Value = _data.DefaultUnitBookmark;
                _pickUpUnitBookmarkVariable.Value = _data.PickUpUnitBookmark;
                _bookFragmentVariable.Value = _data.BookFragments;
                _unitLimitVariable.Value = _data.UnitLimit;

                // 식품부
                _butterVariable.Value = _data.SmoothButter;
                _skewersVariable.Value = _data.RibSkewers;
                _stewVariable.Value = _data.BoneStew;
                _steakVariable.Value = _data.HeartSteak;

                // 가공부
                _powderVariable.Value = _data.Powder;
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            // 기본
            _data.Gold = _goldVariable.Value;
            _data.Essence = _essenceVariable.Value;
            _data.Loot = _lootVariable.Value;

            _data.Action = _actionVariable.Value;

            // 뽑기
            _data.DefaultUnitBookmark = _defaultUnitBookmarkVariable.Value;
            _data.PickUpUnitBookmark = _pickUpUnitBookmarkVariable.Value;
            _data.BookFragments = _bookFragmentVariable.Value;
            _data.UnitLimit = _unitLimitVariable.Value;

            // 식품부
            _data.SmoothButter = _butterVariable.Value;
            _data.RibSkewers = _skewersVariable.Value;
            _data.BoneStew = _stewVariable.Value;
            _data.HeartSteak = _steakVariable.Value;

            // 가공부
            _data.Powder = _powderVariable.Value;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }
    }
}