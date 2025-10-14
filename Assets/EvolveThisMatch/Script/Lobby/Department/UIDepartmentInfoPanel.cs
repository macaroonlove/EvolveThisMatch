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

        private UnityAction<int> _stateControler;

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

            GetButton((int)Buttons.DisposeButton).onClick.AddListener(() => _stateControler?.Invoke(0));
            GetButton((int)Buttons.DepartmentLevelUpButton).onClick.AddListener(() => _stateControler?.Invoke(1));
            GetButton((int)Buttons.BundleGainButton).onClick.AddListener(() => _stateControler?.Invoke(2));
        }

        internal void Initialize(UIDepartmentCanvas departmentCanvas, int totalWeight, UnityAction<int> stateControler)
        {
            _stateControler = stateControler;

            _title.text = departmentCanvas.titleData.DepartmentName;
            _description.text = departmentCanvas.titleData.Description;

            var levelData = departmentCanvas.GetDepartmentLevelData();

            _levelText.text = departmentCanvas.userData.level.ToString();
            _personnelText.text = $"{departmentCanvas.localData.activeJobCount}/{levelData.MaxUnits} 명";
            _storageWeightText.text = $"{totalWeight}/{levelData.StorageWeight} kg";
            _speedText.text = $"{levelData.Speed * 100:F0} %";
        }

        internal void UpdateWeightInfo(UIDepartmentCanvas departmentCanvas, int totalWeight)
        {
            var levelData = departmentCanvas.GetDepartmentLevelData();

            _storageWeightText.text = $"{totalWeight}/{levelData.StorageWeight} kg";
        }
    }
}