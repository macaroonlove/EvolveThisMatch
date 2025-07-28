using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System;
using TMPro;
using UnityEngine;
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

        private DepartmentTemplate _template;
        private UnityAction _action;

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

        internal void Show(DepartmentTemplate template, DepartmentSaveData.Department departmentData, UnityAction action)
        {
            if (template == null) return;

            _action = action;
            _template = template;
            _title.text = template.departmentName;
            _description.text = template.departmentDescription;

            int level = departmentData == null ? 1 : departmentData.level;
            var levelData = template.GetLevelData(level);

            int totalWeight = 0;

            for (int i = 0; i < departmentData.activeJobCount; i++)
            {
                var job = departmentData.GetActiveJob(i);

                TimeSpan elapsed = DateTime.UtcNow - job.startTime;
                float second = (float)elapsed.TotalSeconds;
                var item = template.craftItems[job.craftItemId];
                var unitSpeed = GameDataManager.Instance.profileSaveData.GetAgent(job.chargeUnitId).level + levelData.speed;
                float timePerItem = item.craftTime / unitSpeed;
                int count = Mathf.Min(job.maxAmount, Mathf.FloorToInt(second / timePerItem));
                totalWeight += (int)(item.weight * count);
            }

            _levelText.text = level.ToString();
            _personnelText.text = $"{departmentData.activeJobCount}/{levelData.maxUnits} 명";
            _storageWeightText.text = $"{totalWeight}/{levelData.storageWeight} kg";
            _speedText.text = $"{(levelData.speed * 100).ToString("F0")} %";
        }

        private void OpenDisposePanel()
        {
            _action?.Invoke();
        }

        private void DepartmentLevelUp()
        {

        }

        private void BundleGain()
        {

        }
    }
}