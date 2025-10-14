using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.NetworkTime;
using FrameWork.PlayFabExtensions;
using FrameWork.UI;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private Image _background;

        private UIDepartmentInfoPanel _departmentInfoPanel;
        private UIDisposePanel _disposePanel;
        private List<UIDepartmentItem> _departmentItems;

        private PoolSystem _poolSystem;
        private GameObject _overUICamera;
        private int _totalWeight;

        private UnityAction _onClose;

        private readonly List<GameObject> _spawnedUnits = new List<GameObject>();

        internal DepartmentSaveData.Department userData { get; private set; }
        internal DepartmentData titleData { get; private set; }
        internal DepartmentLocalSaveData localData { get; private set; }

        protected override void Initialize()
        {
            _departmentInfoPanel = GetComponentInChildren<UIDepartmentInfoPanel>();
            _disposePanel = GetComponentInChildren<UIDisposePanel>();
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _overUICamera = Camera.main.transform.Find("OverUICamera").gameObject;

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _background = GetImage((int)Images.Background);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            InitializeDepartmentItems();
        }

        #region 부서 탭 생성
        private async void InitializeDepartmentItems()
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);
            await UniTask.WaitUntil(() => SaveManager.Instance.departmentData.isLoaded);

            var prefab = GetComponentInChildren<UIDepartmentItem>().gameObject;
            var parent = GetObject((int)Objects.DepartmentGroup).transform;
            var departmentTitleDatas = DepartmentSaveDataTemplate.departmentTitleData.Departments;
            var departmentSaveDatas = SaveManager.Instance.departmentData;

            int count = departmentTitleDatas.Count;
            _departmentItems = new List<UIDepartmentItem>(count);

            for (int i = 0; i < count; i++)
            {
                var departmentTitleData = departmentTitleDatas[i];
                var departmentLocalData = departmentSaveDatas.GetDepartmentLocalData(departmentTitleData.DepartmentName);
                var departmentUserData = departmentSaveDatas.GetDepartmentUserData(departmentTitleData.DepartmentName);

                var departmentItem = Instantiate(prefab, parent).GetComponent<UIDepartmentItem>();
                departmentItem.Show(departmentTitleData, () => ChangeDepartment(departmentUserData, departmentTitleData, departmentLocalData));
                _departmentItems.Add(departmentItem);
            }

            Destroy(prefab);
        }

        private void ChangeDepartment(DepartmentSaveData.Department userData, DepartmentData titleData, DepartmentLocalSaveData localData)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _departmentItems) item.DeSelectItem();

            AddressableAssetManager.Instance.GetSprite(titleData.Background, (sprite) =>
            {
                _background.sprite = sprite;
            });

            ShowDepartment(userData, titleData, localData);
        }
        #endregion

        #region Show/Hide
        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            _departmentItems[0].SelectItem();

            _overUICamera.SetActive(true);

            base.Show(true);
        }

        private void Hide()
        {
            _overUICamera.SetActive(false);

            // 유닛 숨기기
            if (_spawnedUnits.Count > 0)
            {
                foreach (var unit in _spawnedUnits)
                {
                    _poolSystem.DeSpawn(unit);
                }
                _spawnedUnits.Clear();
            }

            _onClose?.Invoke();
            Hide(true);
        }
        #endregion

        #region 부서 변경
        /// <summary>
        /// 부서를 변경하거나, 유닛을 배치할 때 호출
        /// </summary>
        private async void ShowDepartment(DepartmentSaveData.Department userData, DepartmentData titleData, DepartmentLocalSaveData localData)
        {
            this.userData = userData;
            this.titleData = titleData;
            this.localData = localData;

            var craftResults = await CalculateCraftResults();

            // 해당 부서에서 작업중인 유닛 보여주기
            SpwanCraftUnit(titleData, localData);

            _totalWeight = 0;
            foreach (var craftResult in craftResults)
            {
                _totalWeight += craftResult.totalWeight;
            }

            // 부서 정보창 최신화
            _departmentInfoPanel.Initialize(this, _totalWeight, ControlInfoPanelState);

            // 배치창 최신화
            _disposePanel.Initialize(this, craftResults, _totalWeight, 
                () => ShowDepartment(userData, titleData, localData), 
                (int newWeight) => ChangeItemWeight(craftResults, newWeight));

            // 제작 재료 Variable 로 보여주기
            RefreshVariableDisplay(titleData);
        }

        /// <summary>
        /// 현재 선택된 부서의 각 작업대 별 생산 결과를 반환하기
        /// </summary>
        private async UniTask<List<CraftResult>> CalculateCraftResults()
        {
            var levelData = GetDepartmentLevelData();
            var now = await NetworkTimeManager.Instance.GetUtcNow();

            // 모든 작업을 시간 순으로 정렬하기 위해 리스트 생성
            var allJobs = new List<(int slotIndex, float finishTime, int weight)>();
            var results = new Dictionary<int, CraftResult>();

            // 현재 부서의 각 작업 불러오기
            for (int i = 0; i < localData.activeJobCount; i++)
            {
                // 작업대 정보 불러오기
                var job = localData.GetActiveJob(i);

                // 작업 중인 아이템
                var item = titleData.CraftItems[job.craftItemId];

                // 작업 중인 유닛의 레벨로 아이템 하나 만드는데 걸리는 시간 계산하기
                float agentLevel = SaveManager.Instance.agentData.GetAgent(job.unitId).level;
                float craftSpeed = agentLevel * 0.01f + levelData.Speed;
                float timePerItem = item.CraftTime / craftSpeed;

                // 작업 경과시간을 계산하기
                TimeSpan elapsed = now - job.startTime;
                float elapsedSeconds = (float)elapsed.TotalSeconds;

                // 시간을 기준으로 생산한 계수 계산하기
                int craftCount = Mathf.Min(job.maxAmount, Mathf.FloorToInt(elapsedSeconds / timePerItem));

                // 재료를 기준으로 생산한 계수 계산하기
                foreach (var required in item.RequiredItems)
                {
                    var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(required.Variable);
                    int craftableCount = variable.Value / required.Amount;

                    craftCount = Mathf.Min(craftCount, craftableCount);
                }

                // 모든 작업의 예상 종료 시간을 저장하기
                for (int j = 0; j < craftCount; j++)
                {
                    float finishTime = (float)(job.startTime.AddSeconds(timePerItem * (j + 1)) - DateTime.UnixEpoch).TotalSeconds;

                    allJobs.Add((i, finishTime, item.Weight));
                }

                // 해당 작업대를 딕셔너리에 추가
                results[i] = new CraftResult(0, 0);
            }

            // 모든 작업을 작업 종료 시간 순으로 정렬
            allJobs.Sort((a, b) => a.finishTime.CompareTo(b.finishTime));

            // 최대 무게 까지만 아이템이 생성되도록 결과 저장
            float usedWeight = 0f;

            foreach (var job in allJobs)
            {
                var result = results[job.slotIndex];

                if (usedWeight + job.weight > levelData.StorageWeight) continue;

                usedWeight += job.weight;
                result.Increment(job.weight);
            }

            return results.Values.ToList();
        }

        /// <summary>
        /// 해당 부서에서 작업중인 유닛 보여주기
        /// </summary>
        private void SpwanCraftUnit(DepartmentData departmentTitleData, DepartmentLocalSaveData departmentLocalData)
        {
            // 기존에 보여주던 유닛 숨기기
            if (_spawnedUnits.Count > 0)
            {
                foreach (var unit in _spawnedUnits)
                {
                    _poolSystem.DeSpawn(unit);
                }
                _spawnedUnits.Clear();
            }

            // 작업중인 유닛 보여주기
            for (int i = 0; i < departmentLocalData.activeJobCount; i++)
            {
                var job = departmentLocalData.GetActiveJob(i);
                var prefab = GameDataManager.Instance.GetAgentTemplateById(job.unitId).overUIPrefab;
                var obj = _poolSystem.Spawn(prefab);
                obj.transform.position = departmentTitleData.UnitPos[i];

                _spawnedUnits.Add(obj);
            }
        }

        /// <summary>
        /// 부서 정보창 버튼 컨트롤러
        /// </summary>
        private void ControlInfoPanelState(int state)
        {
            switch (state)
            {
                case 0:
                    _disposePanel.Show(true);
                    break;
                case 1:
                    break;
                case 2:
                    _disposePanel.BundleGainCraftItem();
                    break;
            }
        }

        /// <summary>
        /// 생산을 완료하거나 생산한 것을 획득할 때 호출
        /// </summary>
        private void ChangeItemWeight(List<CraftResult> craftResults, int newWeight)
        {
            _totalWeight += newWeight;

            // 부서 정보창 최신화
            _departmentInfoPanel.UpdateWeightInfo(this, _totalWeight);

            // 배치창 최신화
            _disposePanel.UpdateDisposePanel(this, _totalWeight);
        }

        /// <summary>
        /// 제작 재료 Variable 로 보여주기
        /// </summary>
        private async void RefreshVariableDisplay(DepartmentData titleData)
        {
            VariableDisplayManager.Instance.HideAll();

            foreach (var showVariable in titleData.ShowVariables)
            {
                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(showVariable);
                VariableDisplayManager.Instance.Show(variable);
            }
        }

        internal DepartmentLevelData GetDepartmentLevelData()
        {
            int level = userData == null ? 1 : userData.level;
            return titleData.GetLevelData(level);
        }
        #endregion
    }

    public readonly struct CraftResult
    {
        public readonly int craftCount;
        public readonly int totalWeight;

        public CraftResult(int craftCount, int totalWeight)
        {
            this.craftCount = craftCount;
            this.totalWeight = totalWeight;
        }

        public CraftResult Increment(int weight)
        {
            return new CraftResult(craftCount + 1, totalWeight + weight);
        }
    }
}