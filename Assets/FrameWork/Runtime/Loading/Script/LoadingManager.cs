using System.Threading.Tasks;
using EvolveThisMatch.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork.Loading
{
    public class LoadingManager : PersistentSingleton<LoadingManager>
    {
        private UILoadingCanvas _uiLoadingCanvas;

        private float _progress = 0f;
        private const float _saveProgressAmount = 0.2f;

        protected override void Initialize()
        {
            _uiLoadingCanvas = GetComponentInChildren<UILoadingCanvas>();
        }

        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1f;

            _uiLoadingCanvas.Show(sceneName, Process);
        }

        private async void Process(string sceneName)
        {
            var saveTask = Save();
            var loadSceneTask = LoadSceneAsync(sceneName);

            await Task.WhenAll(saveTask, loadSceneTask);

            _uiLoadingCanvas.SetProgress(1f);
            loadSceneTask.Result.allowSceneActivation = true;

            await Task.Delay(1000);

            _uiLoadingCanvas.Hide(true);
        }

        #region 데이터 저장
        private async Task Save()
        {
            var progressAmount = _saveProgressAmount / 2;

            Task saveProfile = SaveProfileData(progressAmount);
            Task saveMiniGame = SaveMiniGameData(progressAmount);

            await Task.WhenAll(saveProfile, saveMiniGame);
        }

        private async Task SaveProfileData(float amount)
        {
            await SaveManager.Instance.Save_ProfileData();
            UpdateProgress(amount);
        }

        private async Task SaveMiniGameData(float amount)
        {
            //await SaveManager.Instance.Save_MiniGameData();
            UpdateProgress(amount);

            await Task.CompletedTask;
        }
        #endregion

        #region 씬 로딩
        private async Task<AsyncOperation> LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                float sceneProgress = asyncLoad.progress / 0.9f * (1f - _saveProgressAmount);
                float totalProgress = sceneProgress + _progress;
                _uiLoadingCanvas.SetProgress(totalProgress);

                if (asyncLoad.progress >= 0.9f) break;

                await Task.Yield();
            }

            return asyncLoad;
        }
        #endregion

        private void UpdateProgress(float value)
        {
            _progress += value;
            _uiLoadingCanvas.SetProgress(_progress);
        }
    }
}