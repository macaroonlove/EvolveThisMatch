using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UI;
using FrameWork.UIBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        private int _totalWeight;

        private PoolSystem _poolSystem;
        private GameObject _overUICamera;
        private readonly List<GameObject> _spawnedUnits = new List<GameObject>();

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

        #region 부서 생성
        private async void InitializeDepartmentItems()
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);
            await UniTask.WaitUntil(() => SaveManager.Instance.departmentData.isLoaded);

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
        }

        private void ChangeDepartment(DepartmentTemplate template, DepartmentSaveData.Department departmentData)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _departmentItems) item.DeSelectItem();

            _background.sprite = template.departmentBackground;

            ShowDepartment(template, departmentData);
        }
        #endregion

        public override void Show(bool isForce = false)
        {
            _departmentItems[0].SelectItem();

            _overUICamera.SetActive(true);

            base.Show(isForce);
        }

        /// <summary>
        /// 부서를 변경하거나, 유닛을 배치할 때 호출
        /// </summary>
        private void ShowDepartment(DepartmentTemplate template, DepartmentSaveData.Department departmentData)
        {
            var craftResults = CalculateCraftResults(template, departmentData);

            _totalWeight = 0;
            foreach (var craftResult in craftResults)
            {
                _totalWeight += craftResult.totalWeight;
            }

            // 유닛 숨기기
            if (_spawnedUnits.Count > 0)
            {
                foreach (var unit in _spawnedUnits)
                {
                    _poolSystem.DeSpawn(unit);
                }
                _spawnedUnits.Clear();
            }
            // 유닛 보여주기
            for (int i = 0; i < departmentData.activeJobCount; i++)
            {
                var job = departmentData.GetActiveJob(i);
                var prefab = GameDataManager.Instance.GetAgentTemplateById(job.unitId).overUIPrefab;
                var obj = _poolSystem.Spawn(prefab);
                obj.transform.position = template.unitPos[i];

                _spawnedUnits.Add(obj);
            }

            // 부서 정보창 초기화
            _departmentInfoPanel.Initialize(template, departmentData, _totalWeight, () => _disposePanel.Show(true), () => _disposePanel.BundleGain());

            // 배치창 초기화
            _disposePanel.InitailizeAction(() => ShowDepartment(template, departmentData), (int newWeight) => ChangeItemWeight(template, departmentData, craftResults, newWeight));
            _disposePanel.Initialize(template, departmentData, craftResults, _totalWeight);

            // 제작 재료 보여주기
            VariableDisplayManager.Instance.HideAll();
            foreach (var item in template.craftItems)
            {
                VariableDisplayManager.Instance.Show(item.variable);
            }
        }

        /// <summary>
        /// 생산을 완료하거나 생산한 것을 획득할 때 호출
        /// </summary>
        private void ChangeItemWeight(DepartmentTemplate template, DepartmentSaveData.Department departmentData, List<CraftResult> craftResults, int newWeight)
        {
            _totalWeight += newWeight;

            _departmentInfoPanel.UpdateWeightInfo(template, departmentData, _totalWeight);

            // 배치창 초기화
            _disposePanel.UpdateDisposePanel(craftResults, _totalWeight);
        }

        #region 생산 작업 결과 (패널이 열렸을 때 기준)
        private List<CraftResult> CalculateCraftResults(DepartmentTemplate template, DepartmentSaveData.Department departmentData)
        {
            int level = departmentData == null ? 1 : departmentData.level;
            var levelData = template.GetLevelData(level);

            var now = DateTime.UtcNow;
            var allJobs = new List<(int slotIndex, float nextTime, float interval, int weight)>();
            var results = new Dictionary<int, CraftResult>();

            float usedWeight = 0f;

            for (int i = 0; i < departmentData.activeJobCount; i++)
            {
                var job = departmentData.GetActiveJob(i);
                var item = template.craftItems[job.craftItemId];

                float agentLevel = GameDataManager.Instance.profileSaveData.GetAgent(job.unitId).level;
                float craftSpeed = agentLevel * 0.01f + levelData.speed;
                float timePerItem = item.craftTime / craftSpeed;

                TimeSpan elapsed = now - job.startTime;
                float elapsedSeconds = (float)elapsed.TotalSeconds;

                int maxCount = Mathf.Min(job.maxAmount, Mathf.FloorToInt(elapsedSeconds / timePerItem));

                foreach (var required in item.requiredItems)
                {
                    int ownedCount = required.item.Value;
                    int craftableCount = ownedCount / required.amount;

                    maxCount = Mathf.Min(maxCount, craftableCount);
                }

                for (int j = 0; j < maxCount; j++)
                {
                    float nextTime = (float)(job.startTime.AddSeconds(timePerItem * (j + 1)) - DateTime.UnixEpoch).TotalSeconds;

                    allJobs.Add((i, nextTime, timePerItem, item.weight));
                }

                // 초기 세팅
                results[i] = new CraftResult
                {
                    jobIndex = i,
                    productionCount = 0,
                    totalWeight = 0
                };
            }

            // 생성 예정 시간 순으로 정렬
            allJobs.Sort((a, b) => a.nextTime.CompareTo(b.nextTime));

            foreach (var job in allJobs)
            {
                var result = results[job.slotIndex];

                if (usedWeight + job.weight > levelData.storageWeight)
                    break;

                usedWeight += job.weight;

                result.productionCount++;
                result.totalWeight += job.weight;
            }

            return results.Values.ToList();
        }
        #endregion

        private void Hide()
        {
            VariableDisplayManager.Instance.HideAll();

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

            Hide(true);
        }
    }

    public class CraftResult
    {
        public int jobIndex;
        public int productionCount;
        public int totalWeight;
    }
}