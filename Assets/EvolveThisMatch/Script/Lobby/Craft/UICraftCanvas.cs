using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UICraftCanvas : UIBase
    {
        #region 바인딩
        enum Images
        {
            Background,
        }
        enum Buttons
        {
            CloseButton,
        }
        enum Objects
        {
            DepartmentGroup,
        }
        #endregion

        private Image _background;
        private UIDepartmentInfoPanel _departmentInfoPanel;
        private UIDisposePanel _disposePanel;
        private List<UIDepartmentItem> _departmentItems;

        protected override void Initialize()
        {
            _departmentInfoPanel = GetComponentInChildren<UIDepartmentInfoPanel>();
            _disposePanel = GetComponentInChildren<UIDisposePanel>();

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _background = GetImage((int)Images.Background);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            InitializeDepartmentItems();
        }

        private async void InitializeDepartmentItems()
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            var prefab = GetComponentInChildren<UIDepartmentItem>().gameObject;
            var parent = GetObject((int)Objects.DepartmentGroup).transform;
            var departmentTemplates = LobbyManager.Instance.departments;
            var departmentsDatas = SaveManager.Instance.departmentData.departments;

            int count = departmentTemplates.Count;
            _departmentItems = new List<UIDepartmentItem>(count);

            for (int i = 0; i < count; i++)
            {
                var departmentTemplate = departmentTemplates[i];
                var departmentsData = departmentsDatas.FirstOrDefault(department => department.departmentId == departmentTemplate.departmentName);

                var departmentItem = Instantiate(prefab, parent).GetComponent<UIDepartmentItem>();
                departmentItem.Show(departmentTemplate, () => ChangeDepartment(departmentTemplate, departmentsData));
                _departmentItems.Add(departmentItem);
            }

            Destroy(prefab);

            _departmentItems[0].SelectItem();
        }

        private void ChangeDepartment(DepartmentTemplate template, DepartmentSaveData.Department departmentData)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _departmentItems) item.DeSelectItem();

            _background.sprite = template.departmentBackground;

            _departmentInfoPanel.Show(template, departmentData, () => ShowDisposePanel(template, departmentData));
        }

        private void ShowDisposePanel(DepartmentTemplate template, DepartmentSaveData.Department departmentData)
        {
            _disposePanel.Show(template, departmentData);
        }

        private void Hide()
        {
            Hide(true);
        }
    }
}