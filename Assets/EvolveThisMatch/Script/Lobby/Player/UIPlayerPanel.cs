using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using TMPro;

namespace EvolveThisMatch.Lobby
{
    public class UIPlayerPanel : UIBase
    {
        #region 바인딩
        enum Texts
        {
            PlayerName,
            StatText,
        }
        #endregion

        private TextMeshProUGUI _playerName;
        private TextMeshProUGUI _statText;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _playerName = GetText((int)Texts.PlayerName);
            _statText = GetText((int)Texts.StatText);

            SaveManager.Instance.profileData.changedDisplayName += ShowPlayerName;

            ShowPlayerName();
        }

        private void OnDestroy()
        {
            SaveManager.Instance.profileData.changedDisplayName -= ShowPlayerName;
        }

        private void ShowPlayerName()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                _playerName.text = SaveManager.Instance.profileData.displayName;
            }
            else
            {
                _playerName.text = "테스트중";
            }
        }
    }
}