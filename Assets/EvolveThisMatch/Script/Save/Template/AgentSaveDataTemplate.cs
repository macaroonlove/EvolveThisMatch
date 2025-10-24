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
    #region 유닛 데이터
    [Serializable]
    public class AgentSaveData
    {
        [Tooltip("보유하고 있는 아군 유닛들")]
        public List<Agent> ownedAgents = new List<Agent>();

        #region 데이터 모델
        [Serializable]
        public class Agent
        {
            public int id;
            public int unitCount;
            public int tier;
            public int exp;
            public int level;

            public int seed;
            public Talent[] talent;

            public int selectedSkinId;
            public List<int> ownedSkinIds = new List<int>() { 0 };

            public Agent(int agentId)
            {
                id = agentId;
                unitCount = 0;
                tier = 0;
                exp = 0;
                level = 1;
                selectedSkinId = 0;

                talent = new Talent[5];
            }
        }

        [Serializable]
        public class Talent
        {
            public int id = -1;
            public int value;
            public bool isLock;
        }
        #endregion
    }
    #endregion

    #region 재능 클라이언트 저장 데이터
    [Serializable]
    public class TalentSaveData
    {
        public int agentId;
        public int rollCount;

        public Talent[] finalTalent;
        public List<LockHistory> lockHistory;

        [Serializable]
        public class Talent
        {
            public ObscuredInt id = -1;
            public ObscuredInt value;
            public ObscuredBool isLock;
        }

        [Serializable]
        public class LockHistory
        {
            public ObscuredInt order;
            public ObscuredInt index;
            public ObscuredBool isLock;
        }
    }

    #region 서버 전송 시, 사용할 객체 (복호화용)
    [Serializable]
    public class TalentSaveDataEncrypted
    {
        public int agentId;
        public int rollCount;
        public List<Talent> finalTalent;
        public List<LockHistory> lockHistory;

        [Serializable]
        public class Talent
        {
            public int id;
            public int value;
            public bool isLock;
        }

        [Serializable]
        public class LockHistory
        {
            public int order;
            public int index;
            public bool isLock;
        }

        public static TalentSaveDataEncrypted FromEncrypted(TalentSaveData enc)
        {
            return new TalentSaveDataEncrypted
            {
                agentId = enc.agentId,
                rollCount = enc.rollCount,
                finalTalent = enc.finalTalent
                    .Select(t => new Talent
                    {
                        id = t.id,
                        value = t.value,
                        isLock = t.isLock
                    }).ToList(),
                lockHistory = enc.lockHistory
                    .Select(l => new LockHistory
                    {
                        order = l.order,
                        index = l.index,
                        isLock = l.isLock
                    }).ToList()
            };
        }
    }
    #endregion
    #endregion

    [CreateAssetMenu(menuName = "Templates/SaveData/AgentSaveData", fileName = "AgentSaveData", order = 1)]
    public class AgentSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private AgentSaveData _data;
        private Dictionary<int, TalentSaveData> _talentLocalData = new Dictionary<int, TalentSaveData>();

        private static ObscuredInt[] _agentTierUpRequirements = { 1, 3, 5, 7, 10 };
        private static ObscuredInt[] _agentMaxLevelPerTier = { 50, 70, 100, 120, 150, 180 };
        private static ObscuredInt[] _foodExp = { 30, 500, 2000, 15000 };

        public List<AgentSaveData.Agent> ownedAgents => _data.ownedAgents;

        public static IReadOnlyList<ObscuredInt> foodExp => _foodExp;
        public static TalentTitleData talentTitleData { get; private set; }

        public override void SetDefaultValues()
        {
            _data = new AgentSaveData();

            TitleDataManager.LoadAgentData(ref _agentTierUpRequirements, ref _agentMaxLevelPerTier, ref _foodExp);
            talentTitleData = TitleDataManager.LoadTalentData();

            // 초기 캐릭터 추가
            for (int i = 0; i <= 35; i++)
            {
                AddAgent(i);
            }

            // 서버에도 초기 캐릭터 추가
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "InitializeAgent",
                FunctionParameter = new { count = 35 },
                GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(request, result =>
            {
                JsonObject jsonResult = (JsonObject)result.FunctionResult;

                if ((bool)jsonResult["success"])
                {
                    var agentDataJson = jsonResult["agentData"].ToString();
                    if (!string.IsNullOrEmpty(agentDataJson))
                    {
                        _data = JsonUtility.FromJson<AgentSaveData>(agentDataJson);
                    }
                }
            }, DebugPlayFabError);

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = null;
            _data = JsonUtility.FromJson<AgentSaveData>(json);

            if (_data != null)
            {
                isLoaded = _data.ownedAgents.Count > 0;

                TitleDataManager.LoadAgentData(ref _agentTierUpRequirements, ref _agentMaxLevelPerTier, ref _foodExp);
                talentTitleData = TitleDataManager.LoadTalentData();

                // 로드할 때 데이터가 남아있었다면 즉시 서버로 전송
                LoadTalentLocalData();
                VerifyTalents();
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

        #region 유닛

        #region 유닛 추가 (로컬 적용)
        /// <summary>
        /// 유닛 추가
        /// </summary>
        public void AddAgent(int id, int count = 1)
        {
            if (count <= 0) return;

            var modifyUnit = FindAgent(_data.ownedAgents, id);

            // 유닛이 없었다면 유닛 추가
            if (modifyUnit == null)
            {
                var newUnit = new AgentSaveData.Agent(id);
                newUnit.unitCount = 0;
                _data.ownedAgents.Add(newUnit);
            }
            // 유닛이 있었다면 유닛의 개수 추가
            else
            {
                modifyUnit.unitCount += count;
            }
        }
        #endregion

        #region 유닛 레벨업
        /// <summary>
        /// 유닛 레벨업
        /// </summary>
        public void LevelUpAgent(int id, int[] eatFood, UnityAction onComplete)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "LevelUpAgent",
                FunctionParameter = new { agentId = id, eatFood = eatFood },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {

                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        int finalLevel = Convert.ToInt32(jsonResult["finalLevel"]);
                        int finalExp = Convert.ToInt32(jsonResult["finalExp"]);

                        var agent = FindAgent(ownedAgents, id);
                        agent.exp = finalExp;
                        agent.level = finalLevel;

                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }

        /// <summary>
        /// 해당 티어에서의 최대 레벨 반환
        /// </summary>
        public int GetMaxLevelByTier(int tier)
        {
            if (tier < 0 || tier >= _agentMaxLevelPerTier.Length)
                return _agentMaxLevelPerTier[_agentMaxLevelPerTier.Length - 1];

            return _agentMaxLevelPerTier[tier];
        }
        #endregion

        #region 유닛 승격
        /// <summary>
        /// 유닛 승격
        /// </summary>
        public void TierUpAgent(int id, UnityAction onComplete)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "TierUpAgent",
                FunctionParameter = new { agentId = id },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {

                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        var agent = FindAgent(ownedAgents, id);
                        int requiredCount = _agentTierUpRequirements[agent.tier];

                        agent.unitCount -= requiredCount;
                        agent.tier++;

                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                }, DebugPlayFabError);
        }

        /// <summary>
        /// 승격 가능한 유닛인지 판별
        /// </summary>
        public bool GetTierUpAbleUnit(int id)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, id);

            return modifyUnit != null
                && modifyUnit.tier < _agentTierUpRequirements.Length - 1
                && modifyUnit.unitCount >= _agentTierUpRequirements[modifyUnit.tier];
        }

        /// <summary>
        /// 유닛의 격에 따른 최대 유닛 개수 반환
        /// </summary>
        public int GetMaxUnitCountByTier(int tier)
        {
            if (tier < 0 || tier >= _agentTierUpRequirements.Length)
                return -1;

            return _agentTierUpRequirements[tier];
        }
        #endregion

        #region 재능
        public TalentSaveData GetTalentSaveData(int agentId)
        {
            if (!_talentLocalData.TryGetValue(agentId, out var saveData))
            {
                saveData = new TalentSaveData
                {
                    agentId = agentId,
                    rollCount = 0,
                    finalTalent = new TalentSaveData.Talent[5],
                    lockHistory = new List<TalentSaveData.LockHistory>()
                };

                var agent = FindAgent(ownedAgents, agentId);
                if (agent != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        saveData.finalTalent[i] = new TalentSaveData.Talent();
                        saveData.finalTalent[i].id = agent.talent[i].id;
                        saveData.finalTalent[i].value = agent.talent[i].value;
                        saveData.finalTalent[i].isLock = agent.talent[i].isLock;
                    }
                }

                _talentLocalData[agentId] = saveData;
            }

            return saveData;
        }

        #region 재능 데이터 래퍼
        [Serializable]
        private class TalentSaveDataWrapper
        {
            public List<TalentSaveData> data = new List<TalentSaveData>();
        }

        [System.Serializable]
        public class SeedWrapper
        {
            public List<SeedData> list;
        }

        [System.Serializable]
        public class SeedData
        {
            public int agentId;
            public int seed;
        }
        #endregion

        #region 재능 - 로컬 로드 및 저장
        private void LoadTalentLocalData()
        {
            string json = PlayerPrefs.GetString("TalentData", "{}");
            var wrapper = JsonUtility.FromJson<TalentSaveDataWrapper>(json);

            _talentLocalData = wrapper.data.ToDictionary(x => x.agentId, x => x);
        }

        public void SaveTalentLocalData()
        {
            List<TalentSaveData> saveList = _talentLocalData.Values.ToList();
            string json = JsonUtility.ToJson(new TalentSaveDataWrapper { data = saveList });
            PlayerPrefs.SetString("TalentData", json);
            PlayerPrefs.Save();
        }

        private void ClearTalentLocalData()
        {
            _talentLocalData.Clear();
            PlayerPrefs.DeleteKey("TalentData");
            PlayerPrefs.Save();
        }
        #endregion

        #region 유닛 재능 검증
        /// <summary>
        /// 유닛 재능 검증
        /// </summary>
        public void VerifyTalents(UnityAction onComplete = null)
        {
            if (_talentLocalData.Count == 0) return;
            if (!IsChangedTalent()) return;

            // 동일한 order와 index 기준으로 마지막 잠금 상태만 남기기
            foreach (var localData in _talentLocalData.Values)
            {
                var lockHistory = localData.lockHistory
                    .GroupBy(h => new { h.order, h.index })
                    .Select(g => g.Last())
                    .OrderBy(h => h.order)
                    .ThenBy(h => h.index)
                    .ToList();

                localData.lockHistory = lockHistory;
            }

            var talentLocalData = _talentLocalData.Values.Select(TalentSaveDataEncrypted.FromEncrypted).ToList();

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "VerifyTalents",
                FunctionParameter = new { data = talentLocalData },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        var seedsJson = jsonResult["updatedSeeds"].ToString();
                        var updatedSeeds = JsonUtility.FromJson<SeedWrapper>("{\"list\":" + seedsJson + "}");
                        foreach (var updatedSeed in updatedSeeds.list)
                        {
                            var agent = FindAgent(ownedAgents, updatedSeed.agentId);
                            if (agent != null) agent.seed = updatedSeed.seed;

                            var finalTalent = _talentLocalData[agent.id].finalTalent;
                            for (int i = 0; i < 5; i++)
                            {
                                agent.talent[i].id = finalTalent[i].id;
                                agent.talent[i].value = finalTalent[i].value;
                                agent.talent[i].isLock = finalTalent[i].isLock;
                            }
                        }

                        ClearTalentLocalData();

                        onComplete?.Invoke();
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup("검증에 실패하였습니다.\n고객센터에 문의해주세요.");
                    }
                }, DebugPlayFabError);
        }

        private bool IsChangedTalent()
        {
            foreach (var agent in _data.ownedAgents)
            {
                if (_talentLocalData.ContainsKey(agent.id))
                {
                    var localTalent = _talentLocalData[agent.id].finalTalent;
                    if (agent.talent == null || localTalent == null) continue;
                    int len = Math.Min(agent.talent.Length, localTalent.Length);

                    for (int i = 0; i < len; i++)
                    {
                        if (agent.talent[i].id != localTalent[i].id || agent.talent[i].value != localTalent[i].value || agent.talent[i].isLock != localTalent[i].isLock)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion
        #endregion

        #region 스킨
        /// <summary>
        /// 스킨 추가 (보유 중인 유닛만 스킨 획득 가능)
        /// </summary>
        public void AddAgentSkin(int agentId, int skinId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            // 유닛이 있다면 스킨 추가
            if (modifyUnit != null)
            {
                if (modifyUnit.ownedSkinIds.Contains(skinId))
                {
                    modifyUnit.ownedSkinIds.Add(skinId);
                }
            }
        }

        /// <summary>
        /// 스킨을 보유하고 있는지
        /// </summary>
        public bool IsOwnedAgentSkin(int agentId, int skinId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            if (modifyUnit != null)
            {
                if (skinId == 0) return true;

                return modifyUnit.ownedSkinIds.Contains(skinId);
            }

            return false;
        }

        /// <summary>
        /// 현재 유닛의 모든 스킨ID 받아오기
        /// </summary>
        public List<int> GetAllAgentSkinId(int agentId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            if (modifyUnit != null)
            {
                return modifyUnit.ownedSkinIds;
            }

            return null;
        }

        /// <summary>
        /// 현재 유닛의 선택된 스킨ID 받아오기
        /// </summary>
        public int GetSelectedAgentSkinId(int agentId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            if (modifyUnit != null)
            {
                return modifyUnit.selectedSkinId;
            }

            return -1;
        }
        #endregion

        #region 유틸리티
        public AgentSaveData.Agent GetAgent(int id)
        {
            return FindAgent(_data.ownedAgents, id);
        }

        private AgentSaveData.Agent FindAgent(List<AgentSaveData.Agent> agents, int agentId)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].id == agentId)
                {
                    return agents[i];
                }
            }
            return null;
        }
        #endregion
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