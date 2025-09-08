using Cysharp.Threading.Tasks;
using FrameWork;
using FrameWork.GameSettings;
using FrameWork.PlayFabExtensions;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Save
{
    public enum SaveKey
    {
        Profile,
        Agent,
        Item,
        Department,
        Shop,
        Gacha,
    }

    public class SaveManager : PersistentSingleton<SaveManager>
    {
        [SerializeField] private ProfileSaveDataTemplate _profileData;
        [SerializeField] private AgentSaveDataTemplate _agentData;
        [SerializeField] private ItemSaveDataTemplate _itemData;
        [SerializeField] private FormationSaveDataTemplate _formationData;
        [SerializeField] private DepartmentSaveDataTemplate _departmentData;
        [SerializeField] private ShopSaveDataTemplate _shopData;
        [SerializeField] private GachaSaveDataTemplate _gachaData;

        public ProfileSaveDataTemplate profileData => _profileData;
        public AgentSaveDataTemplate agentData => _agentData;
        public ItemSaveDataTemplate itemData => _itemData;
        public FormationSaveDataTemplate formationData => _formationData;
        public DepartmentSaveDataTemplate departmentData => _departmentData;
        public ShopSaveDataTemplate shopData => _shopData;
        public GachaSaveDataTemplate gachaData => _gachaData;

        private Dictionary<SaveKey, (string key, SaveDataTemplate data)> _saveDataKeys;

        protected override void Initialize()
        {
            // 게임 설정 불러오기
            GameSettingsManager.RestoreSettings();

            // 데이터 지우기
            _profileData.Clear();
            _agentData.Clear();
            _itemData.Clear();
            _formationData.Clear();
            _departmentData.Clear();
            _shopData.Clear();

            InitializeKey();
        }

        private void InitializeKey()
        {
            _saveDataKeys = new Dictionary<SaveKey, (string, SaveDataTemplate)>
            {
                { SaveKey.Profile, ("ProfileData", _profileData) },
                { SaveKey.Agent, ("AgentData", _agentData) },
                { SaveKey.Item, ("ItemData", _itemData) },
                { SaveKey.Department, ("DepartmentData", _departmentData) },
                { SaveKey.Shop, ("ShopData", _shopData) },
                { SaveKey.Gacha, ("GachaData", _gachaData) }
            };
        }

        #region Formation Data
        public bool Load_FormationData()
        {
            bool isSuccess;
            string safeKey = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(profileData.displayName));
            
            isSuccess = LoadPlayerPrefs(_formationData, $"{safeKey}_FormationData");
            
            if (isSuccess == false)
            {
                _formationData.SetDefaultValues();
                Save_FormationData();
            }

            return true;
        }

        public bool Save_FormationData()
        {
            string safeKey = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(profileData.displayName));

            return SavePlayerPrefs(_formationData, $"{safeKey}_FormationData");
        }

        public bool Clear_FormationData()
        {
            string safeKey = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(profileData.displayName));

            return ClearPlayerPrefs($"{safeKey}_FormationData");
        }
        #endregion

        public async UniTask<bool> LoadData(params SaveKey[] saveKeys)
        {
            if (PlayFabAuthService.IsLoginState)
            {
                var datas = new Dictionary<string, SaveDataTemplate>();
                foreach (var saveKey in saveKeys)
                {
                    var (saveName, saveData) = _saveDataKeys[saveKey];
                    datas.Add(saveName, saveData);
                }

                var isSuccess = await LoadPlayFab(datas);

                if (!isSuccess)
                {
                    var missingData = new Dictionary<string, SaveDataTemplate>();

                    foreach (var data in datas)
                    {
                        if (data.Value.isLoaded == false)
                        {
                            data.Value.SetDefaultValues();
                            missingData.Add(data.Key, data.Value);
                        }
                    }

                    if (missingData.Count > 0)
                    {
                        await SavePlayFab(missingData);
                    }
                }
            }
#if UNITY_EDITOR
            else
            {
                foreach (var saveKey in saveKeys)
                {
                    var (key, data) = _saveDataKeys[saveKey];
                    bool isSuccess = LoadPlayerPrefs(data, key);

                    if (!isSuccess)
                    {
                        data.SetDefaultValues();
                        SavePlayerPrefs(data, key);
                    }
                }
            }
#endif

            return true;
        }

        public async UniTask<bool> SaveData(params SaveKey[] saveKeys)
        {
            if (PlayFabAuthService.IsLoginState)
            {
                var datas = new Dictionary<string, SaveDataTemplate>();

                foreach (var saveKey in saveKeys)
                {
                    var (saveName, saveData) = _saveDataKeys[saveKey];
                    datas.Add(saveName, saveData);
                }

                return await SavePlayFab(datas);
            }
            else
            {
                foreach (var saveKey in saveKeys)
                {
                    var (saveName, saveData) = _saveDataKeys[saveKey];
                    SavePlayerPrefs(saveData, saveName);
                }

                return true;
            }
        }

        public async UniTask<bool> ClearData(params SaveKey[] saveKeys)
        {
            if (PlayFabAuthService.IsLoginState)
            {
                var keys = new List<string>();

                foreach (var saveKey in saveKeys)
                {
                    var key = _saveDataKeys[saveKey].key;
                    keys.Add(key);
                }

                return await ClearPlayFab(keys);
            }
            else
            {
                foreach (var saveKey in saveKeys)
                {
                    var key = _saveDataKeys[saveKey].key;
                    ClearPlayerPrefs(key);
                }

                return true;
            }
        }

        #region Load
        private async UniTask<bool> LoadPlayFab(Dictionary<string, SaveDataTemplate> datas)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            var keys = new List<string>(datas.Keys);

            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = PlayFabAuthService.PlayFabId,
                Keys = keys
            }, result =>
            {
                bool allSuccess = true;

                foreach (var key in keys)
                {
                    if (result.Data != null && result.Data.ContainsKey(key))
                    {
                        bool success = datas[key].Load(result.Data[key].Value);
                        allSuccess &= success;
                    }
                    else
                    {
                        allSuccess = false;
                    }
                }

                tcs.TrySetResult(allSuccess);
            }, error =>
            {
                tcs.TrySetResult(false);
            });

            return await tcs.Task;
        }

        private bool LoadPlayerPrefs(SaveDataTemplate data, string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                return data.Load(json);
            }

            return false;
        }
        #endregion

        #region Save
        private async UniTask<bool> SavePlayFab(Dictionary<string, SaveDataTemplate> datas)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            var jsonDatas = new Dictionary<string, string>();
            foreach (var data in datas)
            {
                jsonDatas[data.Key] = data.Value.ToJson();
            }

            var request = new UpdateUserDataRequest { Data = jsonDatas };

            PlayFabClientAPI.UpdateUserData(request,
                result =>
                {
                    tcs.TrySetResult(true);
                }, error =>
                {
                    tcs.TrySetResult(false);
                });

            return await tcs.Task;
        }

        private bool SavePlayerPrefs(SaveDataTemplate data, string key)
        {
            string jsonData = data.ToJson();

            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();

            return true;
        }
        #endregion

        #region Clear
        private async UniTask<bool> ClearPlayFab(List<string> keys)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            var request = new UpdateUserDataRequest { KeysToRemove = keys };

            PlayFabClientAPI.UpdateUserData(request,
                result =>
                {
                    tcs.TrySetResult(true);
                }, error =>
                {
                    tcs.TrySetResult(false);
                });

            return await tcs.Task;
        }

        private bool ClearPlayerPrefs(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();

            return true;
        }
        #endregion
    }
}