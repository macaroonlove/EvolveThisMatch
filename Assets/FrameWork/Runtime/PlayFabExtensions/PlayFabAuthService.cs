using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.PlayFabExtensions
{
    public enum Authtypes
    {
        None,
        Guest,
        EmailAndPassword,
        CraeteAccount,
        Google
    }

    public class PlayFabAuthService
    {
        public static event UnityAction OnDisplayAuthentication;
        public static event UnityAction<LoginResult> OnLoginSuccess;
        public static event UnityAction<PlayFabError> OnPlayFabError;

        public string Email { get; set; }
        public string Password { get; set; }
        public string AuthTicket { get; set; }
        public GetPlayerCombinedInfoRequestParams InfoRequestParams { get; set; }
        public bool ForceLink { get; set; } = false;

        private static PlayFabAuthService _instance;
        public static PlayFabAuthService Instance => _instance ??= new PlayFabAuthService();
        private PlayFabAuthService() { _instance = this; }

        public static string PlayFabId { get; private set; }
        public static string SessionTicket { get; private set; }
        public static bool IsLoginState => !string.IsNullOrEmpty(PlayFabId) && !string.IsNullOrEmpty(SessionTicket);

        private const string LoginRememberKey = "PlayFabLoginRemember";
        private const string PlayFabRememberMeIdKey = "PlayFabIdPassGuid";
        private const string PlayFabAuthTypeKey = "PlayFabAuthType";

        public Authtypes AuthType
        {
            get => (Authtypes)PlayerPrefs.GetInt(PlayFabAuthTypeKey, 0);
            set => PlayerPrefs.SetInt(PlayFabAuthTypeKey, (int)value);
        }

        public bool RememberMe
        {
            get => PlayerPrefs.GetInt(LoginRememberKey, 0) == 1;
            set => PlayerPrefs.SetInt(LoginRememberKey, value ? 1 : 0);
        }

        public string RememberMeId
        {
            get => PlayerPrefs.GetString(PlayFabRememberMeIdKey, "");
            set => PlayerPrefs.SetString(PlayFabRememberMeIdKey, string.IsNullOrEmpty(value) ? Guid.NewGuid().ToString() : value);
        }

        public void ClearRememberMe()
        {
            PlayerPrefs.DeleteKey(LoginRememberKey);
            PlayerPrefs.DeleteKey(PlayFabRememberMeIdKey);
        }

        public void Authenticate(Authtypes authType)
        {
            PlayGamesPlatform.DebugLogEnabled = false;
            AuthType = authType;
            Authenticate();
        }

        public void Authenticate()
        {
            switch (AuthType)
            {
                case Authtypes.None:
                    OnDisplayAuthentication?.Invoke();
                    break;
                case Authtypes.Guest:
                    AuthenticateGuest();
                    break;
                case Authtypes.EmailAndPassword:
                    AuthenticateEmailPassword();
                    break;
                case Authtypes.CraeteAccount:
                    CreateAccount();
                    break;
#if UNITY_ANDROID && !UNITY_EDITOR
                case Authtypes.Google:
                    AuthenticateGooglePlayGames();
                    break;
#endif
            }
        }

        /// <summary>
        /// 게스트로 로그인
        /// </summary>
        private void AuthenticateGuest(Action<LoginResult> onComplete = null)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = InfoRequestParams
            }, result =>
            {
                HandleLoginSuccess(result);
                onComplete?.Invoke(result);
            }, error =>
            {
                onComplete?.Invoke(null);
            });
        }

        /// <summary>
        /// 이메일로 로그인
        /// </summary>
        private void AuthenticateEmailPassword()
        {
            if (RememberMe && !string.IsNullOrEmpty(RememberMeId))
            {
                PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = RememberMeId,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams
                }, HandleLoginSuccess, HandlePlayFabError);
                return;
            }

            if (!RememberMe && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                OnDisplayAuthentication?.Invoke();
                return;
            }

            PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
            {
                TitleId = PlayFabSettings.TitleId,
                Email = Email,
                Password = Password,
                InfoRequestParameters = InfoRequestParams
            }, result =>
            {
                HandleLoginSuccess(result);
                if (RememberMe)
                {
                    RememberMeId = Guid.NewGuid().ToString();
                    AuthType = Authtypes.EmailAndPassword;
                    PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest
                    {
                        CustomId = RememberMeId,
                        ForceLink = ForceLink
                    }, null, null);
                }
            }, HandlePlayFabError);
        }

        /// <summary>
        /// 계정 생성
        /// </summary>
        private void CreateAccount()
        {
            // 게스트 계정을 생성
            AuthenticateGuest(result =>
            {
                if (result == null)
                {
                    HandlePlayFabError(new PlayFabError { Error = PlayFabErrorCode.UnknownError, ErrorMessage = "게스트 로그인 실패.." });
                    return;
                }

                // 게스트 계정을 이메일 계정으로 업그레이드 (현재 로그인된 계정에 이메일과 비밀번호 추가)
                PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
                {
                    Username = result.PlayFabId,
                    Email = Email,
                    Password = Password,
                }, addResult =>
                {
                    AuthenticateEmailPassword();
                }, HandlePlayFabError);
            });
        }

        #region 구글 로그인
        /// <summary>
        /// 구글 플레이 게임즈로 로그인
        /// </summary>
        private void AuthenticateGooglePlayGames()
        {
            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.Instance.Authenticate((status) =>
            {
                Debug.Log($"[GPGS] Authenticate result: {status}");

                if (status == SignInStatus.Success)
                {
                    Debug.Log("[GPGS] 인증 성공, ServerAuthCode 요청 중...");

                    PlayGamesPlatform.Instance.RequestServerSideAccess(false, (serverAuthCode) =>
                    {
                        if (string.IsNullOrEmpty(serverAuthCode))
                        {
                            Debug.LogError("[GPGS] ServerAuthCode가 비어있습니다. Play Console 설정(SHA1, 패키지명) 확인 필요.");
                            HandlePlayFabError(new PlayFabError
                            {
                                Error = PlayFabErrorCode.Unknown,
                                ErrorMessage = "ServerAuthCode 없음 (Play Console 설정 확인)"
                            });
                            return;
                        }

                        Debug.Log($"[GPGS] ServerAuthCode 획득 성공: {serverAuthCode}");

                        AuthTicket = serverAuthCode;

                        PlayFabClientAPI.LoginWithGooglePlayGamesServices(new LoginWithGooglePlayGamesServicesRequest
                        {
                            ServerAuthCode = serverAuthCode,
                            CreateAccount = true,
                            InfoRequestParameters = InfoRequestParams
                        },
                        result =>
                        {
                            Debug.Log("[PlayFab] GPGS 로그인 성공");
                            HandleLoginSuccess(result);
                        },
                        error =>
                        {
                            Debug.LogError($"[PlayFab] 로그인 실패: {error.Error} / {error.ErrorMessage}");
                            HandlePlayFabError(error);
                        });
                    });
                }
                else
                {
                    Debug.LogError($"[GPGS] 로그인 실패 상태: {status}");
                    HandlePlayFabError(new PlayFabError
                    {
                        Error = PlayFabErrorCode.Unknown,
                        ErrorMessage = $"Google Play Games 로그인 실패 (상태: {status})"
                    });
                }
            });
        }

        #endregion

        private void HandleLoginSuccess(LoginResult result)
        {
            PlayFabId = result.PlayFabId;
            SessionTicket = result.SessionTicket;

            TitleDataManager.LoadTitleData(() =>
            {
                OnLoginSuccess?.Invoke(result);
            });
        }

        private void HandlePlayFabError(PlayFabError error)
        {
#if UNITY_EDITOR
            Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
#endif
            OnPlayFabError?.Invoke(error);
        }

        public void Logout()
        {
            PlayFabClientAPI.ForgetAllCredentials();
        }
    }
}