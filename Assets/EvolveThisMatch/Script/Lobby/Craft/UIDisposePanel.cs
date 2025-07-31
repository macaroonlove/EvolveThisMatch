using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        }
        #endregion

        private UIDisposeItem[] _disposeItems;
        private UIDisposeSettingPanel _disposeSettingPanel;

        private DepartmentTemplate _departmentTemplate;
        private DepartmentSaveData.Department _departmentData;

        private UnityAction _updateDepartment;
        private UnityAction<int> _updateInfoPanel;

        private WaitForSeconds _wfs = new WaitForSeconds(0.1f);

        protected override void Initialize()
        {
            _disposeItems = GetComponentsInChildren<UIDisposeItem>();
            _disposeSettingPanel = GetComponentInChildren<UIDisposeSettingPanel>();

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
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
                        float agentLevel = GameDataManager.Instance.profileSaveData.GetAgent(job.chargeUnitId).level;
                        float craftSpeed = agentLevel * 0.01f + levelData.speed;

                        // 해당 작업대에 작업이 있을 경우 초기화
                        _disposeItems[i].Initialize(job, craftItem, craftResult, craftSpeed, () => ShowDisposeSettingPanel(index), _updateInfoPanel);

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
                        float agentLevel = GameDataManager.Instance.profileSaveData.GetAgent(job.chargeUnitId).level;
                        float craftSpeed = agentLevel * 0.01f + levelData.speed;

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
            }
        }

        private IEnumerator UpdateDisposeItems(int i, DepartmentLevelData levelData, CraftItemData craftItem, DepartmentSaveData.CraftingJob job, float craftSpeed)
        {
            // 아이템 1개 만드는데 걸리는 시간
            var timePerItem = craftItem.craftTime / craftSpeed;

            while (true)
            {
                var isUpdateAble = _disposeItems[i].UpdateItem(levelData, craftItem, job, timePerItem);

                if (isUpdateAble == false) break;

                yield return _wfs;
            }
        }

        private void ShowDisposeSettingPanel(int id)
        {
            _disposeSettingPanel.Show(id, _departmentTemplate, _departmentData, _updateDepartment);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            _updateInfoPanel = null;
            _updateDepartment = null;
        }
    }
}