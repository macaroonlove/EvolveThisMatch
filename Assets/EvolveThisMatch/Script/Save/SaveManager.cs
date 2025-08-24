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
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        [SerializeField] private ProfileSaveDataTemplate _profileData;
        [SerializeField] private AgentSaveDataTemplate _agentData;
        [SerializeField] private ItemSaveDataTemplate _itemData;
        [SerializeField] private FormationSaveDataTemplate _formationData;
        [SerializeField] private DepartmentSaveDataTemplate _departmentData;
        [SerializeField] private ShopSaveDataTemplate _shopData;

        public ProfileSaveDataTemplate profileData => _profileData;
        public AgentSaveDataTemplate agentData => _agentData;
        public ItemSaveDataTemplate itemData => _itemData;
        public FormationSaveDataTemplate formationData => _formationData;
        public DepartmentSaveDataTemplate departmentData => _departmentData;
        public ShopSaveDataTemplate shopData => _shopData;

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
        }

        #region 데이터 모음

        #region Profile Data
        public async UniTask<bool> Load_ProfileData()
        {
            bool isSuccess;

            if (PlayFabAuthService.IsLoginState)
            {
                isSuccess = await LoadPlayFab(_profileData, "ProfileData");
            }
            else
            {
                isSuccess = LoadPlayerPrefs(_profileData, "ProfileData");
            }

            if (isSuccess == false)
            {
                _profileData.SetDefaultValues();
                await Save_ProfileData();
            }

            return true;
        }

        public async UniTask<bool> Save_ProfileData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await SavePlayFab(_profileData, "ProfileData");
            }
            else
            {
                return SavePlayerPrefs(_profileData, "ProfileData");
            }
        }

        [ContextMenu("프로필 초기화")]
        public async UniTask<bool> Clear_ProfileData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await ClearPlayFab("ProfileData");
            }
            else
            {
                return ClearPlayerPrefs("ProfileData");
            }
        }
        #endregion

        #region Agent Data
        public async UniTask<bool> Load_AgentData()
        {
            bool isSuccess;
            
            if (PlayFabAuthService.IsLoginState)
            {
                isSuccess = await LoadPlayFab(_agentData, "AgentData");
            }
            else
            {
                isSuccess = LoadPlayerPrefs(_agentData, "AgentData");
            }
            
            if (isSuccess == false)
            {
                _agentData.SetDefaultValues();
                await Save_AgentData();
            }
            
            return true;
        }

        public async UniTask<bool> Save_AgentData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await SavePlayFab(_agentData, "AgentData");
            }
            else
            {
                return SavePlayerPrefs(_agentData, "AgentData");
            }
        }

        [ContextMenu("유닛 데이터 초기화")]
        public async UniTask<bool> Clear_AgentData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await ClearPlayFab("AgentData");
            }
            else
            {
                return ClearPlayerPrefs("AgentData");
            }
        }
        #endregion

        #region Item Data
        public async UniTask<bool> Load_ItemData()
        {
            bool isSuccess;

            if (PlayFabAuthService.IsLoginState)
            {
                isSuccess = await LoadPlayFab(_itemData, "ItemData");
            }
            else
            {
                isSuccess = LoadPlayerPrefs(_itemData, "ItemData");
            }

            if (isSuccess == false)
            {
                _itemData.SetDefaultValues();
                await Save_ItemData();
            }

            return true;
        }

        public async UniTask<bool> Save_ItemData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await SavePlayFab(_itemData, "ItemData");
            }
            else
            {
                return SavePlayerPrefs(_itemData, "ItemData");
            }
        }

        [ContextMenu("아이템 데이터 초기화")]
        public async UniTask<bool> Clear_ItemData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await ClearPlayFab("ItemData");
            }
            else
            {
                return ClearPlayerPrefs("ItemData");
            }
        }
        #endregion

        #region Formation Data
        public bool Load_FormationData()
        {
            bool isSuccess;

            isSuccess = LoadPlayerPrefs(_formationData, "FormationData");

            if (isSuccess == false)
            {
                _formationData.SetDefaultValues();
                Save_FormationData();
            }

            return true;
        }

        public bool Save_FormationData()
        {
            return SavePlayerPrefs(_formationData, "FormationData");
        }

        public bool Clear_FormationData()
        {
            return ClearPlayerPrefs("FormationData");
        }
        #endregion

        #region Department Data
        public async UniTask<bool> Load_DepartmentData()
        {
            bool isSuccess;

            if (PlayFabAuthService.IsLoginState)
            {
                isSuccess = await LoadPlayFab(_departmentData, "DepartmentData");
            }
            else
            {
                isSuccess = LoadPlayerPrefs(_departmentData, "DepartmentData");
            }

            if (isSuccess == false)
            {
                _departmentData.SetDefaultValues();
                await Save_DepartmentData();
            }

            return true;
        }

        public async UniTask<bool> Save_DepartmentData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await SavePlayFab(_departmentData, "DepartmentData");
            }
            else
            {
                return SavePlayerPrefs(_departmentData, "DepartmentData");
            }
        }

        [ContextMenu("부서 초기화")]
        public async UniTask<bool> Clear_DepartmentData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await ClearPlayFab("DepartmentData");
            }
            else
            {
                return ClearPlayerPrefs("DepartmentData");
            }
        }
        #endregion

        #region Shop Data
        public async UniTask<bool> Load_ShopData()
        {
            bool isSuccess;

            if (PlayFabAuthService.IsLoginState)
            {
                isSuccess = await LoadPlayFab(_shopData, "ShopData");
            }
            else
            {
                isSuccess = LoadPlayerPrefs(_shopData, "ShopData");
            }

            if (isSuccess == false)
            {
                _shopData.SetDefaultValues();
                await Save_ShopData();
            }

            return true;
        }

        public async UniTask<bool> Save_ShopData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await SavePlayFab(_shopData, "ShopData");
            }
            else
            {
                return SavePlayerPrefs(_shopData, "ShopData");
            }
        }

        [ContextMenu("상점 초기화")]
        public async UniTask<bool> Clear_ShopData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await ClearPlayFab("ShopData");
            }
            else
            {
                return ClearPlayerPrefs("ShopData");
            }
        }
        #endregion

        #endregion

        #region Load
        private async UniTask<bool> LoadPlayFab(SaveDataTemplate data, string key)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = PlayFabAuthService.PlayFabId,
                Keys = new List<string> { key }
            }, result =>
            {
                bool isSuccess = false;
                
                if (result.Data != null && result.Data.ContainsKey(key))
                {
                    isSuccess = data.Load(result.Data[key].Value);
                }
                else
                {
                    isSuccess = false;
                }
                tcs.TrySetResult(isSuccess);
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
        private async UniTask<bool> SavePlayFab(SaveDataTemplate data, string key)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            string jsonData = data.ToJson();

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> { { key, jsonData } }
            }, result =>
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
        private async UniTask<bool> ClearPlayFab(string key)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                KeysToRemove = new List<string> { key }
            }, result =>
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