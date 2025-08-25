using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.Loading;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

namespace EvolveThisMatch.Login
{
    public class UILoginSceneCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            GuestLoginButton,
            EmailLoginButton,
            StartButton,
            LogoutButton,
        }
        enum CanvasGroupControllers
        {
            LoginButtonGroup,
            StartGroup,
        }
        #endregion

        private UIPlayFabPopupCanvas _uiPlayFabPopupCanvas;
        private UIPlayFabDisplayNameCanvas _uiPlayFabDisplayNameCanvas;

        private Button _guestLoginButton;
        private Button _startButton;

        private CanvasGroupController _startGroup;
        private CanvasGroupController _loginButtonGroup;

        protected override void Initialize()
        {
            _uiPlayFabPopupCanvas = GetComponentInChildren<UIPlayFabPopupCanvas>();
            _uiPlayFabDisplayNameCanvas = GetComponentInChildren<UIPlayFabDisplayNameCanvas>();

            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroupControllers));

            _guestLoginButton = GetButton((int)Buttons.GuestLoginButton);
            _startButton = GetButton((int)Buttons.StartButton);
            _startGroup = GetCanvasGroupController((int)CanvasGroupControllers.StartGroup);
            _loginButtonGroup = GetCanvasGroupController((int)CanvasGroupControllers.LoginButtonGroup);

            GetButton((int)Buttons.EmailLoginButton).onClick.AddListener(EmailLogin);
            GetButton((int)Buttons.LogoutButton).onClick.AddListener(Logout);
            _guestLoginButton.onClick.AddListener(GuestLogin);
            _startButton.onClick.AddListener(GameStart);

            _startGroup.Hide(true);
            _loginButtonGroup.Hide(true);

            PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;
            PlayFabAuthService.OnPlayFabError += OnLoginFailed;
        }

        private void OnDestroy()
        {
            PlayFabAuthService.OnLoginSuccess -= OnLoginSuccess;
            PlayFabAuthService.OnPlayFabError -= OnLoginFailed;
        }

        private void Start()
        {
            var rememberMeId = PlayFabAuthService.Instance.RememberMeId;

            if (string.IsNullOrEmpty(rememberMeId))
            {
                _loginButtonGroup.Show(true);
            }
            else
            {
                _startGroup.Show(true);
            }
        }

        private void GameStart()
        {
            _startButton.interactable = false;
            PlayFabAuthService.Instance.Authenticate();
        }

        private void GuestLogin()
        {
            _guestLoginButton.interactable = false;
            PlayFabAuthService.Instance.Authenticate(Authtypes.Guest);
        }

        private void EmailLogin()
        {
            _uiPlayFabPopupCanvas.Show(true);
        }

        private async void OnLoginSuccess(LoginResult result)
        {
            await SaveManager.Instance.LoadData(SaveKey.Profile, SaveKey.Agent, SaveKey.Item, SaveKey.Department, SaveKey.Shop);

            var profileData = SaveManager.Instance.profileData;

            if (string.IsNullOrEmpty(profileData.displayName))
            {
                _uiPlayFabDisplayNameCanvas.Show(SetDisplayName);
                ResetButtonInteractable();
            }
            else
            {
                if (profileData.isClearTutorial)
                {
                    LoadingManager.Instance.LoadScene("Lobby");
                }
                else
                {
                    TutorialOrLobby(profileData);
                }
            }
        }

        private void OnLoginFailed(PlayFabError result)
        {
            ResetButtonInteractable();
        }

        private void ResetButtonInteractable()
        {
            _guestLoginButton.interactable = true;
            _startButton.interactable = true;
        }

        private async void SetDisplayName(string displayName)
        {
            var profileData = SaveManager.Instance.profileData;

            if (string.IsNullOrEmpty(displayName) == false)
            {
                profileData.displayName = displayName;
                await SaveManager.Instance.SaveData(SaveKey.Profile);
            }

            TutorialOrLobby(profileData);
        }

        private void TutorialOrLobby(ProfileSaveDataTemplate profileData)
        {
            UIPopupManager.Instance.ShowConfirmCancelPopup("튜토리얼 스킵이 가능합니다.\n스킵하시겠습니까?", result =>
            {
                if (result)
                {
                    if (profileData != null)
                    {
                        profileData.isClearTutorial = true;
                    }

                    LoadingManager.Instance.LoadScene("Lobby");
                }
                else
                {
                    LoadingManager.Instance.LoadScene("Tutorial");
                }
            });
        }

        private void Logout()
        {
            _loginButtonGroup.Show(true);
            _startGroup.Hide(true);
        }
    }
}