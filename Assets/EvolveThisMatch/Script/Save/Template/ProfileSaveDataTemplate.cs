using FrameWork.Editor;
using FrameWork.PlayFabExtensions;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using ScriptableObjectArchitecture;
using System;
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

        [Tooltip("유닛 천장")]
        public int UnitLimit;

        [Tooltip("아이템 돋보기")]
        public int Magnifier;

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
        [SerializeField] private ObscuredIntVariable _magnifierVariable;

        [Header("부서_식품")]
        [SerializeField] private ObscuredIntVariable _butterVariable;
        [SerializeField] private ObscuredIntVariable _skewersVariable;
        [SerializeField] private ObscuredIntVariable _stewVariable;
        [SerializeField] private ObscuredIntVariable _steakVariable;

        [Header("부서_가공")]
        [SerializeField] private ObscuredIntVariable _powderVariable;

        private string _displayName;
        public string displayName => _displayName;
        public bool isClearTutorial => _data.isClearTutorial;

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

                LoadDisplayName();

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
                _magnifierVariable.Value = _data.Magnifier;

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
            _data.Magnifier = _magnifierVariable.Value;

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

        #region 닉네임
        private void LoadDisplayName()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                // 계정 이름 불러오기
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                    result =>
                    {
                        _displayName = result.AccountInfo.TitleInfo.DisplayName;
                    },
                    DebugPlayFabError);
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

        public void ChangeDisplayName(string displayName, UnityAction onComplete)
        {
            if (_essenceVariable.Value < 500)
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
                        _essenceVariable.AddValue(-500);
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
}