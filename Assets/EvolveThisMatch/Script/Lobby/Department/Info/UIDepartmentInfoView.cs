using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentInfoView : UIBase
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

        public event UnityAction onOpenDisposeView;
        public event UnityAction onDepartmentLevelUp;
        public event UnityAction onBundleGainItem;

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

            GetButton((int)Buttons.DisposeButton).onClick.AddListener(() => onOpenDisposeView?.Invoke());
            GetButton((int)Buttons.DepartmentLevelUpButton).onClick.AddListener(() => onDepartmentLevelUp?.Invoke());
            GetButton((int)Buttons.BundleGainButton).onClick.AddListener(() => onBundleGainItem?.Invoke());
        }

        internal void Render(DepartmentInfoViewState state)
        {
            _title.text = state.title;
            _description.text = state.description;
            _levelText.text = state.level.ToString();
            _personnelText.text = $"{state.activeCount}/{state.maxUnit} 명";
            _storageWeightText.text = $"{state.totalWeight}/{state.maxWeight} kg";
            _speedText.text = $"{state.speed * 100:F0} %";
        }
    }

    public struct DepartmentInfoViewState
    {
        public string title;
        public string description;
        public int level;
        public int activeCount;
        public int maxUnit;
        public int totalWeight;
        public int maxWeight;
        public float speed;
    }
}