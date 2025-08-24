using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.Events;

#if FACEBOOK
using Facebook.Unity;
#endif

namespace FrameWork.PlayFabExtensions
{
    public enum Authtypes
    {
        None,
        Guest,
        EmailAndPassword,
        CraeteAccount,
        Steam,
        Facebook,
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
                case Authtypes.Steam:
                    AuthenticateSteam();
                    break;
                case Authtypes.Facebook:
                    AuthenticateFacebook();
                    break;
                case Authtypes.Google:
                    AuthenticateGooglePlayGames();
                    break;
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

        /// <summary>
        /// 스팀으로 로그인
        /// </summary>
        private void AuthenticateSteam()
        {
#if STEAM
            if (!string.IsNullOrEmpty(AuthTicket)) 
            {
                //PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
                //{
                //    TitleId = PlayFabSettings.TitleId,
                //    SteamTicket = AuthTicket,
                //    InfoRequestParameters = InfoRequestParams
                //}, result =>
                //{
                //    HandleLoginSuccess(result);
                //}, HandlePlayFabError);
            }
#endif
        }

        /// <summary>
        /// 페이스북으로 로그인
        /// </summary>
        private void AuthenticateFacebook()
        {

#if FACEBOOK
            //if (FB.IsInitialized && FB.IsLoggedIn && !string.IsNullOrEmpty(AuthTicket))
            //{
            //    PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest()
            //    {
            //        TitleId = PlayFabSettings.TitleId,
            //        AccessToken = AuthTicket,
            //        CreateAccount = true,
            //        InfoRequestParameters = InfoRequestParams
            //    }, HandleLoginSuccess, HandlePlayFabError);
            //}
            //else
            //{
            //        OnDisplayAuthentication?.Invoke();
            //}
#endif
        }

        /// <summary>
        /// 구글 플레이 게임즈로 로그인
        /// </summary>
        private void AuthenticateGooglePlayGames()
        {
#if GOOGLEGAMES
            if (!string.IsNullOrEmpty(AuthTicket)) 
            {
                PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    ServerAuthCode = AuthTicket,
                    InfoRequestParameters = InfoRequestParams,
                    CreateAccount = true
                }, HandleLoginSuccess, HandlePlayFabError);
            }
#endif
        }

        private void HandleLoginSuccess(LoginResult result)
        {
            PlayFabId = result.PlayFabId;
            SessionTicket = result.SessionTicket;

            TitleDataManager.LoadTitleData(() => {
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