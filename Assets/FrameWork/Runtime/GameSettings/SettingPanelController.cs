using UnityEngine;

namespace FrameWork.GameSettings
{
    public class SettingPanelController : MonoBehaviour
    {
        private CanvasGroupController canvasGroupController;
        //private CharacterInputs characterInputs;

        private void Awake()
        {
            GameSettingsManager.OnRestoreComplete += OnRestoreComplete;
        }

        private void OnRestoreComplete()
        {
            canvasGroupController = GetComponent<CanvasGroupController>();
            //characterInputs = GameObject.FindWithTag("Player").GetComponent<CharacterInputs>();

            //characterInputs.onSettingChange += OnSettingChange;
        }

        void OnDestroy()
        {
            //characterInputs.onSettingChange -= OnSettingChange;
            GameSettingsManager.OnRestoreComplete -= OnRestoreComplete;
        }

        private void OnSettingChange(bool isOn)
        {
            if (isOn)
            {
                canvasGroupController.Show(true);
            }
            else
            {
                canvasGroupController.Hide();
            }
        }
    }
}