using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork.NetworkTime;
using FrameWork.UIBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIDisposePanel : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            ClearDisposeButton,
        }
        #endregion

        private UIDisposeItem[] _disposeItems;
        private UIDisposeSettingPanel _disposeSettingPanel;

        private DepartmentTemplate _departmentTemplate;
        private DepartmentSaveData.Department _departmentData;

        private UnityAction _updateDepartment;
        private UnityAction<int> _updateInfoPanel;

        private WaitForSecondsRealtime _wfs = new WaitForSecondsRealtime(0.1f);

        protected override void Initialize()
        {
            _disposeItems = GetComponentsInChildren<UIDisposeItem>();
            _disposeSettingPanel = GetComponentInChildren<UIDisposeSettingPanel>();

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.ClearDisposeButton).onClick.AddListener(ClearDispose);
        }

        internal void InitailizeAction(UnityAction updateDepartment, UnityAction<int> updateInfoPanel)
        {
            _updateDepartment = updateDepartment;
            _updateInfoPanel = updateInfoPanel;
        }

        internal void Initialize(DepartmentTemplate departmentTemplate, DepartmentSaveData.Department departmentData, List<CraftResult> craftResults, int panelOpenWeight)
        {
            StopAllCoroutines();

            _departmentTemplate = departmentTemplate;
            _departmentData = departmentData;

            // 레벨 정보 받아오기
            int level = departmentData == null ? 1 : departmentData.level;
            var levelData = departmentTemplate.GetLevelData(level);

            for (int i = 0; i < _disposeItems.Length; i++)
            {
                // 해금된 자리
                if (i < levelData.maxUnits)
                {
                    // 클로저 오류 해결을 위해 새롭게 할당
                    int index = i;

                    // 작업이 있는지 불러오기
                    var job = departmentData.GetActiveJob(i);

                    if (job == null)
                    {
                        // 해당 작업대가 비어있을 때 초기화
                        _disposeItems[i].Initialize(() => ShowDisposeSettingPanel(index));
                    }
                    else
                    {
                        // 패널이 열릴 때까지 진행된 작업 불러오기
                        var craftResult = craftResults[i];

                        // 등록된 작업
                        var craftItem = departmentTemplate.craftItems[job.craftItemId];

                        // 생산 속도
                        float agentLevel = SaveManager.Instance.agentData.GetAgent(job.unitId).level;
                        float craftSpeed = agentLevel * 0.01f + levelData.speed;

                        var timePerItem = craftItem.craftTime / craftSpeed;

                        // 해당 작업대에 작업이 있을 경우 초기화
                        _disposeItems[i].Initialize(job, craftItem, craftResult, craftSpeed,
                            () => ShowDisposeSettingPanel(index), _updateInfoPanel,
                            (int productionCount, float remainTime) => GainCraftItem(job, craftItem, craftResult, productionCount, timePerItem - remainTime),
                            () => RemoveJob(job));

                        // 최대 보관량 >= (현재까지 보관량 + 작업의 아이템 무게) => 즉, 보관이 가능할 때
                        if (levelData.storageWeight >= panelOpenWeight + craftItem.weight)
                        {
                            // 해당 작업대를 동작시키기
                            StartCoroutine(UpdateDisposeItems(i, levelData, craftItem, job, craftSpeed));
                        }
                        else
                        {
                            _disposeItems[i].FullStorageWeight();
                        }
                    }
                }
                // 잠금된 자리
                else
                {
                    // 해금 가능한 레벨 받아오기
                    var unLockIndex = departmentTemplate.GetUnLockMaxUnitLevel(i);
                    _disposeItems[i].Lock(unLockIndex);
                }
            }
        }

        internal void UpdateDisposePanel(List<CraftResult> craftResults, int panelOpenWeight)
        {
            StopAllCoroutines();

            // 레벨 정보 받아오기
            int level = _departmentData == null ? 1 : _departmentData.level;
            var levelData = _departmentTemplate.GetLevelData(level);

            for (int i = 0; i < _disposeItems.Length; i++)
            {
                // 해금된 자리
                if (i < levelData.maxUnits)
                {
                    // 작업이 있는지 불러오기
                    var job = _departmentData.GetActiveJob(i);

                    if (job != null)
                    {
                        // 패널이 열릴 때까지 진행된 작업 불러오기
                        var craftResult = craftResults[i];

                        // 등록된 작업
                        var craftItem = _departmentTemplate.craftItems[job.craftItemId];

                        // 생산 속도
                        float agentLevel = SaveManager.Instance.agentData.GetAgent(job.unitId).level;
                        float craftSpeed = agentLevel * 0.01f + levelData.speed;

                        // 최대 보관량 >= (현재까지 보관량 + 작업의 아이템 무게) => 즉, 보관이 가능할 때
                        if (levelData.storageWeight >= panelOpenWeight + craftItem.weight)
                        {
                            // 필요한 재료가 충분한지 검사
                            foreach (var required in craftItem.requiredItems)
                            {
                                // 충분하지 않다면
                                if (required.item.Value < required.amount)
                                {
                                    _disposeItems[i].LackRequiredItem();

                                    return;
                                }
                            }

                            // 해당 작업대를 동작시키기
                            StartCoroutine(UpdateDisposeItems(i, levelData, craftItem, job, craftSpeed));
                        }
                        else
                        {
                            _disposeItems[i].FullStorageWeight();
                        }
                    }
                }
            }
        }

        private IEnumerator UpdateDisposeItems(int i, DepartmentLevelData levelData, CraftItemData craftItem, DepartmentSaveData.CraftingJob job, float craftSpeed)
        {
            // 아이템 1개 만드는데 걸리는 시간
            var timePerItem = craftItem.craftTime / craftSpeed;

            while (true)
            {
                var isUpdateAble = false;
                var task = _disposeItems[i].UpdateItem(levelData, craftItem, job, timePerItem).ToCoroutine(result => isUpdateAble = result);
                yield return task;

                if (isUpdateAble == false) break;

                yield return _wfs;
            }
        }

        private async void GainCraftItem(DepartmentSaveData.CraftingJob job, CraftItemData craftItem, CraftResult craftResult, int productionCount, float remainTime)
        {
            // 더 이상 생산이 불가능할 경우
            if (productionCount == -1)
            {
                // 현재까지 생산량 받아오기
                productionCount = craftResult.productionCount;
            }

            // 재료별로 가능한 최대 생산량 계산
            foreach (var required in craftItem.requiredItems)
            {
                int maxProductionCountByItem = required.item.Value / required.amount;
                productionCount = Mathf.Min(productionCount, maxProductionCountByItem);
            }

            // 최대 생산량 넘는거 방지
            productionCount = Mathf.Min(productionCount, job.maxAmount);

            // 생산이 불가능했었을 경우
            if (productionCount <= 0) return;

            // 아이템 획득
            craftItem.variable.AddValue(productionCount);

            // 재료 소진
            foreach (var required in craftItem.requiredItems)
            {
                required.item.AddValue(-(required.amount * productionCount));
            }

            // 생산량 차감
            job.maxAmount -= productionCount;

            // 전부 생산했다면
            if (job.maxAmount <= 0)
            {
                // 작업대 비우기
                _departmentData.RemoveActiveJob(job);
            }
            else
            {
                // 시작 시간 보정하기
                var currentTime = await NetworkTimeManager.Instance.GetUtcNow();
                job.startTime = currentTime - TimeSpan.FromSeconds(remainTime);
            }

            // 부서 업데이트
            _updateDepartment?.Invoke();

            // 저장
            _ = SaveManager.Instance.SaveData(SaveKey.Department, SaveKey.Profile);
        }

        private void RemoveJob(DepartmentSaveData.CraftingJob job)
        {
            // 작업대 비우기
            _departmentData.RemoveActiveJob(job);

            // 부서 업데이트
            _updateDepartment?.Invoke();

            // 저장
            _ = SaveManager.Instance.SaveData(SaveKey.Department);
        }

        private void ShowDisposeSettingPanel(int id)
        {
            _disposeSettingPanel.Show(id, _departmentTemplate, _departmentData, () =>
            {
                _disposeItems[id].GainCraftItem();
                _updateDepartment?.Invoke();
            });
        }

        internal void BundleGain()
        {
            foreach (var item in _disposeItems)
            {
                item.GainCraftItem();
            }
        }

        private void ClearDispose()
        {
            foreach (var item in _disposeItems)
            {
                item.RemoveJob();
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            _updateInfoPanel = null;
            _updateDepartment = null;
        }
    }
}