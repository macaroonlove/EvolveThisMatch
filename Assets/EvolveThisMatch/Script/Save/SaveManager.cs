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
        [SerializeField] private FormationSaveDataTemplate _formationData;

        public ProfileSaveDataTemplate profileData => _profileData;
        public FormationSaveDataTemplate formationData => _formationData;

        protected override void Initialize()
        {
            // 게임 설정 불러오기
            GameSettingsManager.RestoreSettings();

            // 데이터 지우기
            profileData.Clear();
            formationData.Clear();
        }

        #region Profile Data
        public async UniTask<bool> Load_ProfileData()
        {
            bool isSuccess;

            if (PlayFabAuthService.IsLoginState)
            {
                isSuccess = await LoadPlayFab(profileData, "ProfileData");
            }
            else
            {
                isSuccess = LoadPlayerPrefs(profileData, "ProfileData");
            }

            if (isSuccess == false)
            {
                profileData.SetDefaultValues();
                await Save_ProfileData();
            }

            return true;
        }

        public async UniTask<bool> Save_ProfileData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await SavePlayFab(profileData, "ProfileData");
            }
            else
            {
                return SavePlayerPrefs(profileData, "ProfileData");
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

        #region Formation Data
        public async UniTask<bool> Load_FormationData()
        {
            bool isSuccess;

            if (PlayFabAuthService.IsLoginState)
            {
                isSuccess = await LoadPlayFab(formationData, "FormationData");
            }
            else
            {
                isSuccess = LoadPlayerPrefs(formationData, "FormationData");
            }

            if (isSuccess == false)
            {
                formationData.SetDefaultValues();
                await Save_FormationData();
            }

            return true;
        }

        public async UniTask<bool> Save_FormationData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await SavePlayFab(formationData, "FormationData");
            }
            else
            {
                return SavePlayerPrefs(formationData, "FormationData");
            }
        }

        public async UniTask<bool> Clear_FormationData()
        {
            if (PlayFabAuthService.IsLoginState)
            {
                return await ClearPlayFab("FormationData");
            }
            else
            {
                return ClearPlayerPrefs("FormationData");
            }
        }
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
                    isSuccess = true;
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