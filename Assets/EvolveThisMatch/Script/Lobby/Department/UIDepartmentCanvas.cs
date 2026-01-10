using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UI;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentCanvas : UIBase
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

        [SerializeField] private GameObject _prefab;

        private Image _background;

        private PoolSystem _poolSystem;
        private GameObject _overUICamera;

        private UIDepartmentPresenter _presenter;

        private UIDepartmentInfoView _infoView;
        private UIDepartmentDisposeView _disposeView;
        private UIDepartmentDisposeRegistView _disposeRegistView;
        private List<UIDepartmentListItem> _departmentListItems;

        private UnityAction _onClose;

        private readonly List<GameObject> _spawnedUnits = new List<GameObject>();

        protected override void Initialize()
        {
            var model = new UIDepartmentModel();
            _presenter = new UIDepartmentPresenter(this, model);

            _infoView = GetComponentInChildren<UIDepartmentInfoView>();
            _disposeView = GetComponentInChildren<UIDepartmentDisposeView>();
            _disposeRegistView = GetComponentInChildren<UIDepartmentDisposeRegistView>();

            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _overUICamera = Camera.main.transform.Find("OverUICamera").gameObject;

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _background = GetImage((int)Images.Background);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            InitializeDepartmentItems();
            InitializeEvent();
        }

        #region 부서 탭 생성
        private async void InitializeDepartmentItems()
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);
            await UniTask.WaitUntil(() => SaveManager.Instance.departmentData.isLoaded);

            var parent = GetObject((int)Objects.DepartmentGroup).transform;
            var departmentTitleDatas = DepartmentSaveDataTemplate.departmentTitleData.Departments;
            var departmentSaveDatas = SaveManager.Instance.departmentData;

            int count = departmentTitleDatas.Count;
            _departmentListItems = new List<UIDepartmentListItem>(count);

            for (int i = 0; i < count; i++)
            {
                var departmentTitleData = departmentTitleDatas[i];
                var departmentLocalData = departmentSaveDatas.GetDepartmentLocalData(departmentTitleData.DepartmentName);
                var departmentUserData = departmentSaveDatas.GetDepartmentUserData(departmentTitleData.DepartmentName);

                var departmentListItem = Instantiate(_prefab, parent).GetComponent<UIDepartmentListItem>();
                departmentListItem.Show(departmentTitleData, () => _presenter?.ChangeDepartment(departmentUserData, departmentTitleData, departmentLocalData));
                _departmentListItems.Add(departmentListItem);
            }
        }
        #endregion

        #region 이벤트 초기화
        private void InitializeEvent()
        {
            _infoView.onOpenDisposeView += OpenDisposeView;
            _disposeView.onOpenDepartmentDisposeRegistView += OpenDepartmentDisposeRegistView;

            _disposeView.onCreateCraftItem += CreateCraftItem;
            _infoView.onDepartmentLevelUp += DepartmentLevelUp;

            _disposeView.onGainItem += GainItem;
            _infoView.onBundleGainItem += BundleGainItem;

            _disposeRegistView.onRegistJob += RegistJob;
            _disposeView.onRemoveJob += RemoveJob;
            _disposeView.onClearJob += ClearJob;
        }
        #endregion

        #region Show/Hide
        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            // 첫 번째 부서 선택
            _departmentListItems[0].SelectItem();

            _overUICamera.SetActive(true);
            base.Show(true);
        }

        private void Hide()
        {
            _disposeView.StopTick();

            // 유닛 숨기기
            ClearCraftUnit();

            _onClose?.Invoke();

            _overUICamera.SetActive(false);
            Hide(true);
        }
        #endregion

        #region 렌더링
        public void DeselectItems()
        {
            // 모든 아이템 선택 취소
            foreach (var item in _departmentListItems) item.DeSelectItem();
        }

        public void Render(DepartmentViewState state)
        {
            // 해당 부서 배경 보여주기
            _background.sprite = state.background;

            // 해당 부서에서 작업중인 유닛 보여주기
            SpwanCraftUnit(state.titleData, state.localData);

            // 제작 재료 Variable 로 보여주기
            RefreshVariableDisplay(state.titleData);

            // 부서 정보 렌더링
            _infoView.Render(state.infoState);

            // 배치 정보 초기화
            _disposeView.Initialize(state.snapshot);

            // 배치 등록 창 초기화
            _disposeRegistView.Bind(state.titleData, state.localData);

            // 배치 등록 창 숨기기
            _disposeRegistView.Hide(true);
        }

        #region 유닛 관리
        private void SpwanCraftUnit(DepartmentData titleData, DepartmentLocalSaveData localData)
        {
            // 기존에 보여주던 유닛 숨기기
            ClearCraftUnit();

            // 작업중인 유닛 보여주기
            for (int i = 0; i < localData.workbenchCount; i++)
            {
                var job = localData.GetJob(i);
                if (job.isActive)
                {
                    var prefab = GameDataManager.Instance.GetAgentTemplateById(job.unitId).overUIPrefab;
                    var obj = _poolSystem.Spawn(prefab);
                    obj.transform.position = titleData.UnitPos[i];

                    _spawnedUnits.Add(obj);
                }
            }
        }

        private void ClearCraftUnit()
        {
            if (_spawnedUnits.Count > 0)
            {
                foreach (var unit in _spawnedUnits)
                {
                    _poolSystem.DeSpawn(unit);
                }
                _spawnedUnits.Clear();
            }
        }
        #endregion

        private void RefreshVariableDisplay(DepartmentData titleData)
        {
            VariableDisplayManager.Instance.HideAll();

            foreach (var showVariable in titleData.ShowVariables)
            {
                var variable = SaveManager.Instance.profileData.GetVariable(showVariable);
                VariableDisplayManager.Instance.Show(variable);
            }
        }

        public void UpdateInfoRender(DepartmentInfoViewState viewState)
        {
            _infoView.Render(viewState);
        }
        #endregion

        #region 콜백 메서드
        private void OpenDisposeView()
        {
            _disposeView.Show(true);
        }

        private void OpenDepartmentDisposeRegistView(int index)
        {
            _disposeRegistView.Show(index);
        }

        private void CreateCraftItem()
        {
            _presenter?.UpdateInfoViewState();
        }

        private void DepartmentLevelUp()
        {
            // TODO: 부서 레벨업
        }

        private void GainItem(int index)
        {
            _presenter?.GainItem(index);
        }

        private void BundleGainItem()
        {
            _presenter?.BundleGainItem();
        }

        private void RegistJob(int index, int agentId, int itemId, int count)
        {
            _presenter?.RegistJob(index, agentId, itemId, count);
        }

        private void RemoveJob(int index)
        {
            _presenter?.RemoveJob(index);
        }

        private void ClearJob()
        {
            _presenter?.ClearJob();
        }
        #endregion
    }

    public struct DepartmentViewState
    {
        public readonly DepartmentInfoViewState infoState;
        public readonly DepartmentSnapshot snapshot;

        public readonly Sprite background;
        public readonly DepartmentData titleData;
        public readonly DepartmentLocalSaveData localData;

        public DepartmentViewState(DepartmentInfoViewState infoState, DepartmentSnapshot snapshot, Sprite background, DepartmentData titleData, DepartmentLocalSaveData localData)
        {
            this.infoState = infoState;
            this.snapshot = snapshot;
            this.background = background;
            this.titleData = titleData;
            this.localData = localData;
        }
    }
}