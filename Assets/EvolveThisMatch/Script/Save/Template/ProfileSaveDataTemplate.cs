using Cysharp.Threading.Tasks;
using FrameWork.Editor;
using FrameWork.PlayFabExtensions;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class ProfileSaveData
    {
        [Tooltip("튜토리얼 클리어 여부")]
        public bool isClearTutorial;

        // =========================
        // 기본
        // =========================
        [Tooltip("골드")]
        public int Gold;

        [Tooltip("세계석")]
        public int Essence;

        [Tooltip("봉인된 서약서")]
        public int Action;

        [Tooltip("전리품")]
        public int Loot;


        // =========================
        // 식품부
        // =========================
        [Tooltip("괴이한 고깃덩어리")]
        public int ChunkMeat;

        [Tooltip("말캉버터")]
        public int SmoothButter;

        [Tooltip("갈빗살 꼬치")]
        public int RibSkewers;

        [Tooltip("골수 스튜")]
        public int BoneStew;

        [Tooltip("하트빔 스테이크")]
        public int HeartSteak;


        // =========================
        // 연금부
        // =========================
        [Tooltip("희미한 인과력")]
        public int Causality;

        [Tooltip("세계의 흔적")]
        public int Powder;

        [Tooltip("창세의 코인")]
        public int GenesisCoin;

        [Tooltip("기원의 파편")]
        public int OriginCrystal;

        [Tooltip("운명의 룬석")]
        public int FateRoneStone;

        [Tooltip("영웅의 인장")]
        public int HeroSeal;


        // =========================
        // 건축부
        // =========================
        [Tooltip("변질된 근육 줄기")]
        public int MuscleStem;

        [Tooltip("거친 근육 섬유")]
        public int RawMuscleFiber;

        [Tooltip("정제된 근육 섬유")]
        public int PurifiedMuscleFiber;

        [Tooltip("엮은 근육 줄기")]
        public int BoundMuscleStrand;

        [Tooltip("단단한 근육 블록")]
        public int HardenedMuscleBlock;

        [Tooltip("강화 바리케이드 재료")]
        public int ReinforcedBarrier;


        // =========================
        // 연구부
        // =========================
        [Tooltip("부셔진 크로니클")]
        public int BrokenChronicle;


        // =========================
        // 뽑기
        // =========================
        [Tooltip("픽업 유닛 책갈피")]
        public int PickUpUnitBookmark;

        [Tooltip("일반 유닛 책갈피")]
        public int DefaultUnitBookmark;

        [Tooltip("찢어진 책 조각")]
        public int BookFragments;

        [Tooltip("돋보기")]
        public int Magnifier;
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/ProfileSaveData", fileName = "ProfileSaveData", order = 0)]
    public class ProfileSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private ProfileSaveData _data;

        [SerializeField] private List<VariableEntry> _variables = new List<VariableEntry>();
        private Dictionary<EVariableType, ObscuredIntVariable> _variableDic;

        private string _displayName;
        public string displayName => _displayName;
        public bool isClearTutorial => _data.isClearTutorial;

        #region 저장 및 로드
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

                _variableDic = _variables.ToDictionary(v => v.key, v => v.variable);

                // 기본
                _variableDic[EVariableType.Gold].Value = _data.Gold;
                _variableDic[EVariableType.Essence].Value = _data.Essence;
                _variableDic[EVariableType.Action].Value = _data.Action;
                _variableDic[EVariableType.Loot].Value = _data.Loot;

                // 식품부
                _variableDic[EVariableType.ChunkMeat].Value = _data.ChunkMeat;
                _variableDic[EVariableType.SmoothButter].Value = _data.SmoothButter;
                _variableDic[EVariableType.RibSkewers].Value = _data.RibSkewers;
                _variableDic[EVariableType.BoneStew].Value = _data.BoneStew;
                _variableDic[EVariableType.HeartSteak].Value = _data.HeartSteak;

                // 연금부
                _variableDic[EVariableType.Causality].Value = _data.Causality;
                _variableDic[EVariableType.Powder].Value = _data.Powder;
                _variableDic[EVariableType.GenesisCoin].Value = _data.GenesisCoin;
                _variableDic[EVariableType.OriginCrystal].Value = _data.OriginCrystal;
                _variableDic[EVariableType.FateRoneStone].Value = _data.FateRoneStone;
                _variableDic[EVariableType.HeroSeal].Value = _data.HeroSeal;

                // 건축부
                _variableDic[EVariableType.MuscleStem].Value = _data.MuscleStem;
                _variableDic[EVariableType.RawMuscleFiber].Value = _data.RawMuscleFiber;
                _variableDic[EVariableType.PurifiedMuscleFiber].Value = _data.PurifiedMuscleFiber;
                _variableDic[EVariableType.BoundMuscleStrand].Value = _data.BoundMuscleStrand;
                _variableDic[EVariableType.HardenedMuscleBlock].Value = _data.HardenedMuscleBlock;
                _variableDic[EVariableType.ReinforcedBarrier].Value = _data.ReinforcedBarrier;

                // 연구부
                _variableDic[EVariableType.BrokenChronicle].Value = _data.BrokenChronicle;

                // 뽑기
                _variableDic[EVariableType.PickUpUnitBookmark].Value = _data.PickUpUnitBookmark;
                _variableDic[EVariableType.DefaultUnitBookmark].Value = _data.DefaultUnitBookmark;
                _variableDic[EVariableType.BookFragments].Value = _data.BookFragments;
                _variableDic[EVariableType.Magnifier].Value = _data.Magnifier;
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            // 기본
            _data.Gold = _variableDic[EVariableType.Gold].Value;
            _data.Essence = _variableDic[EVariableType.Essence].Value;
            _data.Action = _variableDic[EVariableType.Action].Value;
            _data.Loot = _variableDic[EVariableType.Loot].Value;

            // 식품부
            _data.ChunkMeat = _variableDic[EVariableType.ChunkMeat].Value;
            _data.SmoothButter = _variableDic[EVariableType.SmoothButter].Value;
            _data.RibSkewers = _variableDic[EVariableType.RibSkewers].Value;
            _data.BoneStew = _variableDic[EVariableType.BoneStew].Value;
            _data.HeartSteak = _variableDic[EVariableType.HeartSteak].Value;

            // 연금부
            _data.Causality = _variableDic[EVariableType.Causality].Value;
            _data.Powder = _variableDic[EVariableType.Powder].Value;
            _data.GenesisCoin = _variableDic[EVariableType.GenesisCoin].Value;
            _data.OriginCrystal = _variableDic[EVariableType.OriginCrystal].Value;
            _data.FateRoneStone = _variableDic[EVariableType.FateRoneStone].Value;
            _data.HeroSeal = _variableDic[EVariableType.HeroSeal].Value;

            // 건축부
            _data.MuscleStem = _variableDic[EVariableType.MuscleStem].Value;
            _data.RawMuscleFiber = _variableDic[EVariableType.RawMuscleFiber].Value;
            _data.PurifiedMuscleFiber = _variableDic[EVariableType.PurifiedMuscleFiber].Value;
            _data.BoundMuscleStrand = _variableDic[EVariableType.BoundMuscleStrand].Value;
            _data.HardenedMuscleBlock = _variableDic[EVariableType.HardenedMuscleBlock].Value;
            _data.ReinforcedBarrier = _variableDic[EVariableType.ReinforcedBarrier].Value;

            // 연구부
            _data.BrokenChronicle = _variableDic[EVariableType.BrokenChronicle].Value;

            // 뽑기
            _data.PickUpUnitBookmark = _variableDic[EVariableType.PickUpUnitBookmark].Value;
            _data.DefaultUnitBookmark = _variableDic[EVariableType.DefaultUnitBookmark].Value;
            _data.BookFragments = _variableDic[EVariableType.BookFragments].Value;
            _data.Magnifier = _variableDic[EVariableType.Magnifier].Value;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }
        #endregion

        public List<(ObscuredIntVariable, int)> ChangeProfileData(string json)
        {
            var results = new List<(ObscuredIntVariable, int)>();

            var data = JsonUtility.FromJson<ProfileSaveData>(json);

            void CompareAndUpdate(EVariableType type, ref int oldValue, int newValue)
            {
                if (oldValue != newValue)
                {
                    int diff = newValue - oldValue;
                    oldValue = newValue;
                    _variableDic[type].Value = newValue;
                    results.Add((_variableDic[type], diff));
                }
            }

            // 기본
            CompareAndUpdate(EVariableType.Gold, ref _data.Gold, data.Gold);
            CompareAndUpdate(EVariableType.Essence, ref _data.Essence, data.Essence);
            CompareAndUpdate(EVariableType.Action, ref _data.Action, data.Action);
            CompareAndUpdate(EVariableType.Loot, ref _data.Loot, data.Loot);

            // 식품부
            CompareAndUpdate(EVariableType.ChunkMeat, ref _data.ChunkMeat, data.ChunkMeat);
            CompareAndUpdate(EVariableType.SmoothButter, ref _data.SmoothButter, data.SmoothButter);
            CompareAndUpdate(EVariableType.RibSkewers, ref _data.RibSkewers, data.RibSkewers);
            CompareAndUpdate(EVariableType.BoneStew, ref _data.BoneStew, data.BoneStew);
            CompareAndUpdate(EVariableType.HeartSteak, ref _data.HeartSteak, data.HeartSteak);

            // 연금부
            CompareAndUpdate(EVariableType.Causality, ref _data.Causality, data.Causality);
            CompareAndUpdate(EVariableType.Powder, ref _data.Powder, data.Powder);
            CompareAndUpdate(EVariableType.GenesisCoin, ref _data.GenesisCoin, data.GenesisCoin);
            CompareAndUpdate(EVariableType.OriginCrystal, ref _data.OriginCrystal, data.OriginCrystal);
            CompareAndUpdate(EVariableType.FateRoneStone, ref _data.FateRoneStone, data.FateRoneStone);
            CompareAndUpdate(EVariableType.HeroSeal, ref _data.HeroSeal, data.HeroSeal);

            // 건축부
            CompareAndUpdate(EVariableType.MuscleStem, ref _data.MuscleStem, data.MuscleStem);
            CompareAndUpdate(EVariableType.RawMuscleFiber, ref _data.RawMuscleFiber, data.RawMuscleFiber);
            CompareAndUpdate(EVariableType.PurifiedMuscleFiber, ref _data.PurifiedMuscleFiber, data.PurifiedMuscleFiber);
            CompareAndUpdate(EVariableType.BoundMuscleStrand, ref _data.BoundMuscleStrand, data.BoundMuscleStrand);
            CompareAndUpdate(EVariableType.HardenedMuscleBlock, ref _data.HardenedMuscleBlock, data.HardenedMuscleBlock);
            CompareAndUpdate(EVariableType.ReinforcedBarrier, ref _data.ReinforcedBarrier, data.ReinforcedBarrier);

            // 연구부
            CompareAndUpdate(EVariableType.BrokenChronicle, ref _data.BrokenChronicle, data.BrokenChronicle);

            // 뽑기
            CompareAndUpdate(EVariableType.PickUpUnitBookmark, ref _data.PickUpUnitBookmark, data.PickUpUnitBookmark);
            CompareAndUpdate(EVariableType.DefaultUnitBookmark, ref _data.DefaultUnitBookmark, data.DefaultUnitBookmark);
            CompareAndUpdate(EVariableType.BookFragments, ref _data.BookFragments, data.BookFragments);
            CompareAndUpdate(EVariableType.Magnifier, ref _data.Magnifier, data.Magnifier);

            return results;
        }

        public ObscuredIntVariable GetVariable(string name)
        {
            if (Enum.TryParse<EVariableType>(name, out var type))
            {
                return _variableDic[type];
            }

            return null;
        }

        #region 닉네임
        internal async UniTask LoadDisplayName()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                var tcs = new UniTaskCompletionSource<string>();

                // 계정 이름 불러오기
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                    result =>
                    {
                        tcs.TrySetResult(result.AccountInfo.TitleInfo.DisplayName);
                        
                    },
                    error =>
                    {
                        DebugPlayFabError(error);
                        tcs.TrySetResult(string.Empty);
                    });

                _displayName = await tcs.Task;
            }
#if UNITY_EDITOR
            else
            {
                if (PlayerPrefs.HasKey("DisplayName"))
                {
                    _displayName = PlayerPrefs.GetString("DisplayName");
                }
            }
#endif
        }

        public void SaveDisplayName(string displayName, UnityAction<bool> onComplete)
        {
            if (PlayFabAuthService.IsLoginState)
            {
                var request = new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = displayName
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(request,
                    result =>
                    {
                        _displayName = displayName;
                        onComplete?.Invoke(true);
                    },
                    error =>
                    {
                        onComplete?.Invoke(false);
                        DebugPlayFabError(error);
                    });
            }
#if UNITY_EDITOR
            else
            {
                PlayerPrefs.SetString("DisplayName", displayName);
                PlayerPrefs.Save();
            }
#endif
        }

        /// <summary>
        /// 닉네임 변경
        /// </summary>
        public void ChangeDisplayName(string displayName, UnityAction onComplete)
        {
            if (_variableDic[EVariableType.Essence].Value < 500)
            {
                UIPopupManager.Instance.ShowConfirmPopup("세계석이 부족합니다.");
                return;
            }

            SaveDisplayName(displayName, (isComplete) => 
            {
                if (isComplete)
                {
                    PayEssenceVariable();
                    onComplete?.Invoke();
                }
            });
        }

        /// <summary>
        /// 지불하기 (획득 불가)
        /// </summary>
        private void PayEssenceVariable()
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "PayVariable",
                FunctionParameter = new { variable = "Essence", amount = 500 },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        _variableDic[EVariableType.Essence].AddValue(-500);
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }
        #endregion

        #region 튜토리얼
        public void ClearTutorial()
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "ClearTutorial",
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if (!(bool)jsonResult["success"])
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }
        #endregion

        private void DebugPlayFabError(PlayFabError error)
        {
            switch (error.Error)
            {
                case PlayFabErrorCode.ConnectionError:
                case PlayFabErrorCode.ExperimentationClientTimeout:
                    UIPopupManager.Instance.ShowConfirmPopup("네트워크 연결을 확인해주세요.", () =>
                    {
                        SceneManager.LoadScene("Login");
                    });
                    break;

                case PlayFabErrorCode.ServiceUnavailable:
                    UIPopupManager.Instance.ShowConfirmPopup("게임 서버가 불안정합니다.\n나중에 다시 접속해주세요.\n죄송합니다.", () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    });
                    break;

                case PlayFabErrorCode.NameNotAvailable:
                    UIPopupManager.Instance.ShowConfirmPopup("이미 존재하는 닉네임입니다. 다른 이름을 입력해주세요.");
                    break;

                case PlayFabErrorCode.InvalidUsername:
                    UIPopupManager.Instance.ShowConfirmPopup("닉네임 규칙에 맞지 않습니다. 다시 입력해주세요.");
                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
#endif
                    break;
            }
        }
    }



    [Serializable]
    public class VariableEntry
    {
        public EVariableType key;
        public ObscuredIntVariable variable;
    }

    public enum EVariableType
    {
        Gold,
        Essence,
        Action,
        Loot,
        ChunkMeat,
        SmoothButter,
        RibSkewers,
        BoneStew,
        HeartSteak,
        Causality,
        Powder,
        GenesisCoin,
        OriginCrystal,
        FateRoneStone,
        HeroSeal,
        MuscleStem,
        RawMuscleFiber,
        PurifiedMuscleFiber,
        BoundMuscleStrand,
        HardenedMuscleBlock,
        ReinforcedBarrier,
        BrokenChronicle,
        PickUpUnitBookmark,
        DefaultUnitBookmark,
        BookFragments,
        Magnifier
    }
}