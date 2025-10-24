using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.NetworkTime;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
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

        private string _departmentId;
        private UnityAction _updateDepartment;

        private List<Coroutine> _craftWorkbenchs = new List<Coroutine>();
        private WaitForSecondsRealtime _wfs = new WaitForSecondsRealtime(0.1f);

        protected override void Initialize()
        {
            _disposeItems = GetComponentsInChildren<UIDisposeItem>();
            _disposeSettingPanel = GetComponentInChildren<UIDisposeSettingPanel>();

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.ClearDisposeButton).onClick.AddListener(ClearJob);
        }

        internal void Initialize(UIDepartmentCanvas departmentCanvas, List<CraftResult> craftResults, int panelOpenWeight, UnityAction updateDepartment, UnityAction<int> completeCraft)
        {
            _departmentId = departmentCanvas.userData.departmentId;
            _updateDepartment = updateDepartment;

            // 배치 설정창 닫기
            _disposeSettingPanel.Hide(true);

            // 이전에 실행되던 코루틴 멈추기
            for (int i = 0; i < _craftWorkbenchs.Count; i++)
            {
                var coroutine = _craftWorkbenchs[i];
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    _craftWorkbenchs[i] = null;
                }
            }

            // 레벨 정보 받아오기
            var levelData = departmentCanvas.GetDepartmentLevelData();

            for (int i = 0; i < _disposeItems.Length; i++)
            {
                // 해금된 작업대라면
                if (i < levelData.MaxUnits)
                {
                    // 클로저 오류 해결을 위해 새롭게 할당
                    int index = i;

                    // 작업이 있는지 불러오기
                    var job = departmentCanvas.localData.GetActiveJob(i);

                    // 해당 작업대가 비어있다면
                    if (job == null)
                    {
                        _disposeItems[i].Initialize(() => ShowDisposeSettingPanel(departmentCanvas, index));
                    }
                    // 비어있지 않다면
                    else
                    {
                        // 패널이 열릴 때까지 진행된 작업 불러오기
                        var craftResult = craftResults[i];
                        
                        // 등록된 작업
                        var craftItem = departmentCanvas.titleData.CraftItems[job.craftItemId];

                        // 생산 속도
                        float agentLevel = SaveManager.Instance.agentData.GetAgent(job.unitId).level;
                        float craftSpeed = agentLevel * 0.01f + levelData.Speed;

                        var timePerItem = craftItem.CraftTime / craftSpeed;

                        // 해당 작업대에 작업이 있을 경우 초기화
                        _disposeItems[i].Initialize(job, craftItem, craftResult, craftSpeed,
                            () => ShowDisposeSettingPanel(departmentCanvas, index),
                            completeCraft,
                            (int craftCount, float remainTime) => GainCraftItem(job.workbenchId, craftCount, timePerItem - remainTime, craftResult),
                            () => RemoveJob(departmentCanvas.localData, job));

                        // 최대 보관량 >= (현재까지 보관량 + 작업의 아이템 무게) => 즉, 보관이 가능할 때
                        if (levelData.StorageWeight >= panelOpenWeight + craftItem.Weight)
                        {
                            // 해당 작업대를 동작시키기
                            var coroutine = StartCoroutine(UpdateDisposeItems(i, levelData, craftItem, job, craftSpeed));

                            if (_craftWorkbenchs.Count <= i)
                                _craftWorkbenchs.Add(coroutine);
                            else
                                _craftWorkbenchs[i] = coroutine;
                        }
                        else
                        {
                            _disposeItems[i].FullStorageWeight();
                        }
                    }
                }
                // 잠금된 작업대라면
                else
                {
                    // 해금 가능한 레벨 받아오기
                    var unLockIndex = departmentCanvas.titleData.GetUnLockMaxUnitLevel(i);
                    _disposeItems[i].Lock(unLockIndex);
                }
            }
        }

        /// <summary>
        /// 같은 부서의 다른 아이템이 완성되었다면, 조건부로 작업을 멈춰주는 역할
        /// </summary>
        internal async void UpdateDisposePanel(UIDepartmentCanvas departmentCanvas, int totalWeight)
        {
            // 레벨 정보 받아오기
            var levelData = departmentCanvas.GetDepartmentLevelData();

            for (int i = 0; i < _disposeItems.Length; i++)
            {
                // 해금된 작업대라면
                if (i < levelData.MaxUnits)
                {
                    // 작업대가 비어있다면 넘기기
                    var job = departmentCanvas.localData.GetActiveJob(i);
                    if (job == null) continue;

                    // 등록된 작업
                    var craftItem = departmentCanvas.titleData.CraftItems[job.craftItemId];

                    // 최대 보관량 > (현재까지 보관량 + 해당 작업대에서 생산중인 아이템 1개의 무게)
                    bool canCraft = levelData.StorageWeight > totalWeight + craftItem.Weight;

                    if (canCraft)
                    {
                        // 필요한 재료가 충분한지 검사
                        foreach (var required in craftItem.RequiredItems)
                        {
                            var requiredItem = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(required.Variable);
                            if (requiredItem.Value < required.Amount)
                            {
                                // 재료가 충분하지 않다면
                                _disposeItems[i].LackRequiredItem();
                                canCraft = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // 보관량이 가득 찼다면
                        _disposeItems[i].FullStorageWeight();
                    }

                    // 생산이 불가능한 라인이라면
                    if (canCraft == false)
                    {
                        if (_craftWorkbenchs[i] != null)
                        {
                            StopCoroutine(_craftWorkbenchs[i]);
                            _craftWorkbenchs[i] = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 작업대 동작시키는 코루틴
        /// </summary>
        private IEnumerator UpdateDisposeItems(int i, DepartmentLevelData levelData, DepartmentCraftData craftItem, DepartmentLocalSaveData.CraftingJob job, float craftSpeed)
        {
            // 아이템 1개 만드는데 걸리는 시간
            var timePerItem = craftItem.CraftTime / craftSpeed;

            while (true)
            {
                // 다음 생산품 제작 가능 여부
                var isUpdateAble = false;

                // 생산품 제작 시도
                var task = _disposeItems[i].UpdateItem(levelData, craftItem, job, timePerItem).ToCoroutine(result => isUpdateAble = result);
                yield return task;

                // 다음 아이템 제작이 불가능하다면
                if (isUpdateAble == false) break;

                // 잠시 대기
                yield return _wfs;
            }
        }

        /// <summary>
        /// 배치 설정창 열기
        /// </summary>
        private void ShowDisposeSettingPanel(UIDepartmentCanvas departmentCanvas, int id)
        {
            _disposeSettingPanel.Show(id, departmentCanvas, () =>
            {
                _disposeItems[id].GainCraftItem();
                _updateDepartment?.Invoke();
            });
        }

        #region 생산품 획득
        /// <summary>
        /// 생산품 개별 획득
        /// </summary>
        private async void GainCraftItem(int workbenchId, int craftCount, float remainTime, CraftResult craftResult)
        {
            // 더 이상 생산이 불가능할 경우
            if (craftCount == -1)
            {
                // 현재까지 생산량 받아오기
                craftCount = craftResult.craftCount;
            }

            var utcNow = await NetworkTimeManager.Instance.GetUtcNow();

            // 아이템 획득 시도하기
            SaveManager.Instance.departmentData.GainCraftItem(_departmentId, workbenchId, craftCount, utcNow, remainTime, () =>
            {
                // 부서 업데이트
                _updateDepartment?.Invoke();
            });
        }

        /// <summary>
        /// 부서의 생산품 일괄 획득
        /// </summary>
        internal async void BundleGainCraftItem()
        {
            List<int> craftCounts = new List<int>();
            List<float> remainTimes = new List<float>();
            foreach (var item in _disposeItems)
            {
                var data = item.GetDataToSendServer();
                craftCounts.Add(data.Item1);
                remainTimes.Add(data.Item2);
            }

            var utcNow = await NetworkTimeManager.Instance.GetUtcNow();

            // 아이템 획득 시도하기
            SaveManager.Instance.departmentData.BundleGainCraftItem(_departmentId, craftCounts, utcNow, remainTimes, () =>
            {
                // 부서 업데이트
                _updateDepartment?.Invoke();
            });
        }
        #endregion

        #region 작업대 비우기
        /// <summary>
        /// 작업대 비우기
        /// </summary>
        private void RemoveJob(DepartmentLocalSaveData localData, DepartmentLocalSaveData.CraftingJob job)
        {
            // 작업대의 아이템 획득
            _disposeItems[job.workbenchId].GainCraftItem();

            // 작업대 비우기
            localData.RemoveActiveJob(job);

            // 부서 업데이트
            _updateDepartment?.Invoke();

            // 저장
            SaveManager.Instance.departmentData.SaveDepartmentLocalData();
        }

        /// <summary>
        /// 부서의 작업대 일괄 비우기
        /// </summary>
        private void ClearJob()
        {
            // 생산한 아이템이 모두 획득
            BundleGainCraftItem();

            // 작업대 비우기
            foreach (var item in _disposeItems)
            {
                item.RemoveJob();
            }
        }
        #endregion

        private void OnDestroy()
        {
            StopAllCoroutines();

            _updateDepartment = null;
        }
    }
}