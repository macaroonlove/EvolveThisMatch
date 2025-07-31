using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentInfoPanel : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Title,
            Description,
            LevelText,
            PersonnelText,
            StorageWeightText,
            SpeedText,
        }
        enum Buttons
        {
            DisposeButton,
            DepartmentLevelUpButton,
            BundleGainButton
        }
        #endregion

        private TextMeshProUGUI _title;
        private TextMeshProUGUI _description;
        private TextMeshProUGUI _levelText;
        private TextMeshProUGUI _personnelText;
        private TextMeshProUGUI _storageWeightText;
        private TextMeshProUGUI _speedText;

        private UnityAction _openDisposePanelAction;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _title = GetText((int)Texts.Title);
            _description = GetText((int)Texts.Description);
            _levelText = GetText((int)Texts.LevelText);
            _personnelText = GetText((int)Texts.PersonnelText);
            _storageWeightText = GetText((int)Texts.StorageWeightText);
            _speedText = GetText((int)Texts.SpeedText);

            GetButton((int)Buttons.DisposeButton).onClick.AddListener(OpenDisposePanel);
            GetButton((int)Buttons.DepartmentLevelUpButton).onClick.AddListener(DepartmentLevelUp);
            GetButton((int)Buttons.BundleGainButton).onClick.AddListener(BundleGain);
        }

        internal void Initialize(DepartmentTemplate template, DepartmentSaveData.Department departmentData, int totalWeight, UnityAction openDisposePanelAction)
        {
            if (template == null) return;

            _openDisposePanelAction = openDisposePanelAction;
            _title.text = template.departmentName;
            _description.text = template.departmentDescription;

            int level = departmentData == null ? 1 : departmentData.level;
            var levelData = template.GetLevelData(level);

            _levelText.text = level.ToString();
            _personnelText.text = $"{departmentData.activeJobCount}/{levelData.maxUnits} 명";
            _storageWeightText.text = $"{totalWeight}/{levelData.storageWeight} kg";
            _speedText.text = $"{levelData.speed * 100:F0} %";
        }

        internal void UpdateWeightInfo(DepartmentTemplate template, DepartmentSaveData.Department departmentData, int totalWeight)
        {
            int level = departmentData == null ? 1 : departmentData.level;
            var levelData = template.GetLevelData(level);

            _storageWeightText.text = $"{totalWeight}/{levelData.storageWeight} kg";
        }

        private void OpenDisposePanel()
        {
            _openDisposePanelAction?.Invoke();
        }

        private void DepartmentLevelUp()
        {

        }

        private void BundleGain()
        {

        }
    }
}