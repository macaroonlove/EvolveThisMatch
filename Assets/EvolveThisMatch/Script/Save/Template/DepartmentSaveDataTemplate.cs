using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.Editor;
using FrameWork.PlayFabExtensions;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EvolveThisMatch.Save
{
    #region 서버 저장
    [Serializable]
    public class DepartmentSaveData
    {
        [Tooltip("각 부서 정보")]
        public List<Department> departments = new List<Department>();

        [Serializable]
        public class Department
        {
            public string departmentId;
            public int level;
        }
    }
    #endregion

    #region 로컬 저장
    [Serializable]
    public class DepartmentLocalSaveData
    {
        [SerializeField] private string _departmentId;
        [SerializeField] private List<CraftingJob> _activeJobs = new List<CraftingJob>();

        public string departmentId => _departmentId;
        internal IReadOnlyList<CraftingJob> activeJobs => _activeJobs;
        public int activeJobCount => _activeJobs.Count;

        public DepartmentLocalSaveData(string name)
        {
            _departmentId = name;
        }

        public CraftingJob GetActiveJob(int workbenchId)
        {
            return _activeJobs.FirstOrDefault(job => job.workbenchId == workbenchId);
        }

        public void SetActiveJob(int workbenchId, int unitId, int itemId, int maxAmount)
        {
            // 해당 작업대가 존재할리 없다면
            if (workbenchId < 0 || workbenchId > 2) return;

            var job = GetActiveJob(workbenchId);

            // 작업대가 존재하지 않으면 새로운 작업 추가
            if (job == null)
            {
                job = new CraftingJob();
                job.workbenchId = workbenchId;

                _activeJobs.Add(job);
            }

            job.unitId = unitId;
            job.craftItemId = itemId;
            job.maxAmount = maxAmount;
            job.startTime = DateTime.UtcNow;
        }

        public void RemoveActiveJob(CraftingJob job)
        {
            if (job == null) return;

            _activeJobs.Remove(job);
        }

        [Serializable]
        public class CraftingJob
        {
            public ObscuredInt unitId;
            public ObscuredInt workbenchId;
            public ObscuredInt craftItemId;
            public ObscuredInt maxAmount;
            [SerializeField] private ObscuredLong _startTimeTicks;

            internal long startTimeTicks => _startTimeTicks;

            public DateTime startTime
            {
                get => new DateTime(_startTimeTicks);
                set => _startTimeTicks = value.Ticks;
            }
        }
    }

    #region 서버 전송 시, 사용할 객체 (복호화용)
    [Serializable]
    public class DepartmentLocalSaveDataEncrypted
    {
        public string departmentId;
        public List<CraftingJob> activeJobs = new List<CraftingJob>();

        [Serializable]
        public class CraftingJob
        {
            public int unitId;
            public int workbenchId;
            public int craftItemId;
            public int maxAmount;
            public long startTimeTicks;
        }

        public static DepartmentLocalSaveDataEncrypted FromEncrypted(DepartmentLocalSaveData localData)
        {
            return new DepartmentLocalSaveDataEncrypted
            {
                departmentId = localData.departmentId,
                activeJobs = localData.activeJobs
                    .Select(job => new CraftingJob
                    {
                        unitId = job.unitId,
                        workbenchId = job.workbenchId,
                        craftItemId = job.craftItemId,
                        maxAmount = job.maxAmount,
                        startTimeTicks = job.startTimeTicks
                    }).ToList()
            };
        }
    }
    #endregion
    #endregion

    [CreateAssetMenu(menuName = "Templates/SaveData/DepartmentSaveData", fileName = "DepartmentSaveData", order = 3)]
    public class DepartmentSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private DepartmentSaveData _data;
        private Dictionary<string, DepartmentLocalSaveData> _departmentLocalSaveData = new Dictionary<string, DepartmentLocalSaveData>();

        public static DepartmentTitleData departmentTitleData { get; private set; }

        #region 저장 및 로드
        public override void SetDefaultValues()
        {
            _data = new DepartmentSaveData();

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<DepartmentSaveData>(json);

            if (_data != null)
            {
                isLoaded = _data.departments.Count > 0;

                departmentTitleData = TitleDataManager.LoadDepartmentData();

                LoadDepartmentLocalData();
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }

        public DepartmentSaveData.Department GetDepartmentUserData(string name)
        {
            // 기존 부서 검색
            var userData = _data.departments.Find(d => d.departmentId == name);
            if (userData != null)
                return userData;

            // 없으면 새로 생성
            userData = new DepartmentSaveData.Department
            {
                departmentId = name,
                level = 1
            };

            _data.departments.Add(userData);
            return userData;
        }
        #endregion

        #region Local Data
        public DepartmentLocalSaveData GetDepartmentLocalData(string name)
        {
            if (!_departmentLocalSaveData.TryGetValue(name, out var saveData))
            {
                saveData = new DepartmentLocalSaveData(name);

                _departmentLocalSaveData[name] = saveData;
            }

            return saveData;
        }

        #region 부서 로컬 로드 및 저장
        private void LoadDepartmentLocalData()
        {
            string json = PlayerPrefs.GetString("DepartmentData", "{}");
            var wrapper = JsonUtility.FromJson<DepartmentSaveDataWrapper>(json);

            foreach (var localData in wrapper.data)
                _departmentLocalSaveData[localData.departmentId] = localData;
        }

        public void SaveDepartmentLocalData()
        {
            List<DepartmentLocalSaveData> saveList = _departmentLocalSaveData.Values.ToList();
            string json = JsonUtility.ToJson(new DepartmentSaveDataWrapper { data = saveList });
            PlayerPrefs.SetString("DepartmentData", json);
            PlayerPrefs.Save();
        }

        [Serializable]
        private class DepartmentSaveDataWrapper
        {
            public List<DepartmentLocalSaveData> data = new List<DepartmentLocalSaveData>();
        }
        #endregion
        #endregion

        #region 생산품 획득
        public void GainCraftItem(string departmentId, int workbenchId, int craftCount, float remainTime, UnityAction onComplete)
        {
            var localData = _departmentLocalSaveData.TryGetValue(departmentId, out var departmentLocalSaveData) ? DepartmentLocalSaveDataEncrypted.FromEncrypted(departmentLocalSaveData) : null;

            if (localData == null) return;

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "GainCraftItem",
                FunctionParameter = new { departmentId = departmentId, workbenchId = workbenchId, localData = localData },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        var job = departmentLocalSaveData.GetActiveJob(workbenchId);

                        // 시작 시간 보정
                        var currentTimeStr = jsonResult["currentTime"]?.ToString();
                        if (DateTime.TryParse(currentTimeStr, out DateTime currentTime))
                            job.startTime = currentTime - TimeSpan.FromSeconds(remainTime);

                        // 생산 개수 감소
                        int craftCount = Convert.ToInt32(jsonResult["craftCount"]);
                        job.maxAmount = Math.Max(0, job.maxAmount - craftCount);

                        // 변경된 생산품 적용
                        SaveManager.Instance.profileData.ChangeProfileData(jsonResult["profileData"].ToString());

                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }

        public void BundleGainCraftItem(string departmentId, List<float> remainTimes, UnityAction onComplete)
        {
            var localData = _departmentLocalSaveData.TryGetValue(departmentId, out var departmentLocalSaveData) ? DepartmentLocalSaveDataEncrypted.FromEncrypted(departmentLocalSaveData) : null;

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "BundleGainCraftItem",
                FunctionParameter = new { departmentId = departmentId, localData = localData },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        // 시작 시간 불러오기
                        var currentTimeStr = jsonResult["currentTime"]?.ToString();
                        if (DateTime.TryParse(currentTimeStr, out DateTime currentTime))
                        {
                            currentTime = DateTime.UtcNow;
                        }

                        // 모든 작업대 순회
                        if (jsonResult.TryGetValue("workbenches", out var workbenchObj) && workbenchObj is JsonObject workbenches)
                        {
                            foreach (var kvp in workbenches)
                            {
                                int workbenchId = Convert.ToInt32(kvp.Key);
                                int craftCount = Convert.ToInt32(kvp.Value);

                                var job = departmentLocalSaveData.GetActiveJob(workbenchId);

                                // 시작 시간 보정
                                job.startTime = currentTime - TimeSpan.FromSeconds(remainTimes[workbenchId]);

                                // 생산 개수 감소
                                job.maxAmount = Math.Max(0, job.maxAmount - craftCount);
                            }
                        }

                        // 변경된 생산품 적용
                        SaveManager.Instance.profileData.ChangeProfileData(jsonResult["profileData"].ToString());

                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }
        #endregion

        private void DebugPlayFabError(PlayFabError error)
        {
            switch (error.Error)
            {
                case PlayFabErrorCode.ConnectionError:
                case PlayFabErrorCode.ExperimentationClientTimeout:
                    UIPopupManager.Instance.ShowConfirmPopup("네트워크 연결을 확인해주세요.", () =>
                    {
                        SceneManager.LoadScene("Login");
                    });
                    break;
                case PlayFabErrorCode.ServiceUnavailable:
                    UIPopupManager.Instance.ShowConfirmPopup("게임 서버가 불안정합니다.\n나중에 다시 접속해주세요.\n죄송합니다.", () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    });
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
#endif
                    break;
            }
        }
    }
}