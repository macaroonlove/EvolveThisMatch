using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine;

namespace FrameWork.GameSettings
{
    [AddComponentMenu("GameSettings/UI/DisplayName Text")]
    public class DisplayNameText : UIBase
    {
        #region 바인딩
        enum Texts
        {
            DisplayNameText,
        }
        enum InputFields
        {
            DisplayNameInputField,
        }
        enum Buttons
        {
            ModifyButton,
            CancelButton,
            SaveButton,
        }
        enum CanvasGroups
        {
            DefaultPanel,
            ModifyPanel,
        }
        #endregion

        private TextMeshProUGUI _displayNameText;
        private TMP_InputField _displayNameInputField;
        private CanvasGroupController _defaultPanel;
        private CanvasGroupController _modifyPanel;

        protected async override void Initialize()
        {
            BindText(typeof(Texts));
            BindInputField(typeof(InputFields));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroups));

            _displayNameText = GetText((int)Texts.DisplayNameText);
            _displayNameInputField = GetInputField((int)InputFields.DisplayNameInputField);
            _defaultPanel = GetCanvasGroupController((int)CanvasGroups.DefaultPanel);
            _modifyPanel = GetCanvasGroupController((int)CanvasGroups.ModifyPanel);

            GetButton((int)Buttons.ModifyButton).onClick.AddListener(ShowModifyPanel);
            GetButton((int)Buttons.CancelButton).onClick.AddListener(HideModifyPanel);
            GetButton((int)Buttons.SaveButton).onClick.AddListener(SaveDisplayName);

            await UniTask.WaitUntil(() => SaveManager.Instance.profileData.isLoaded);

            _displayNameText.text = SaveManager.Instance.profileData.displayName;
        }

        private void ShowModifyPanel()
        {
            _defaultPanel.Hide(true);
            _modifyPanel.Show(true);
        }

        private void HideModifyPanel()
        {
            _modifyPanel.Hide(true);
            _defaultPanel.Show(true);

            _displayNameInputField.text = "";
        }

        private void SaveDisplayName()
        {
            var result = _displayNameInputField.text;

            if (result.Length >= 2 && result.Length <= 10)
            {
                UIPopupManager.Instance.ShowConfirmCancelPopup($"<sprite name=essence> 500개를 사용해\n닉네임을 {result}로\n변경하시겠습니까?", (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        SaveManager.Instance.profileData.ChangeDisplayName(result, () => {
                            _displayNameText.text = result;
                            HideModifyPanel();
                        });
                    }
                });
            }
            else
            {
                UIPopupManager.Instance.ShowConfirmPopup("닉네임은 2글자 이상 10글자 이하여야 합니다.");
            }
        }
    }
}