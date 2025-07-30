using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIDisposePanel : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        private UIDisposeItem[] _disposeItems;
        private UIDisposeSettingPanel _disposeSettingPanel;

        protected override void Initialize()
        {
            _disposeItems = GetComponentsInChildren<UIDisposeItem>();
            _disposeSettingPanel = GetComponentInChildren<UIDisposeSettingPanel>();

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
        }

        internal void Show(DepartmentTemplate template, DepartmentSaveData.Department departmentData)
        {
            int level = departmentData == null ? 1 : departmentData.level;
            var levelData = template.GetLevelData(level);

            for (int i = 0; i < _disposeItems.Length; i++)
            {
                int index = i;
                if (i < levelData.maxUnits)
                {
                    var job = departmentData.GetActiveJob(index);
                    _disposeItems[index].Show(template, job, levelData, () => ShowDisposeSettingPanel(index, template, departmentData, levelData));
                }
                else
                {
                    var unLockIndex = template.GetUnLockMaxUnitLevel(index);
                    _disposeItems[index].Lock(unLockIndex);
                }
            }

            Show(true);
        }

        private void ShowDisposeSettingPanel(int id, DepartmentTemplate template, DepartmentSaveData.Department departmentData, DepartmentLevelData levelData)
        {
            _disposeSettingPanel.Show(id, template, departmentData, () => ConfirmDisposeItem(id, template, departmentData, levelData));
        }

        private void ConfirmDisposeItem(int id, DepartmentTemplate template, DepartmentSaveData.Department departmentData, DepartmentLevelData levelData)
        {
            var job = departmentData.GetActiveJob(id);
            
            _disposeItems[id].Show(template, job, levelData, () => ShowDisposeSettingPanel(id, template, departmentData, levelData));
        }
    }
}