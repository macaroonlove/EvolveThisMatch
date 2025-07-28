using EvolveThisMatch.Save;
using FrameWork.UIBinding;

namespace EvolveThisMatch.Lobby
{
    public class UIDisposePanel : UIBase
    {
        private UIDisposeItem[] _disposeItems;

        protected override void Initialize()
        {
            _disposeItems = GetComponentsInChildren<UIDisposeItem>();
        }

        internal void Show(DepartmentTemplate template, DepartmentSaveData.Department departmentData)
        {
            int level = departmentData == null ? 1 : departmentData.level;
            var levelData = template.GetLevelData(level);

            for (int i = 0; i < _disposeItems.Length; i++)
            {
                if (i < levelData.maxUnits)
                {
                    var job = departmentData.GetActiveJob(i);
                    _disposeItems[i].Show(template, job, levelData);
                }
                else
                {
                    var unLockIndex = template.GetUnLockMaxUnitLevel(i);
                    _disposeItems[i].Lock(unLockIndex);
                }
            }

            base.Show(false);
        }
    }
}