using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace FrameWork.GameSettings
{
    [AddComponentMenu("GameSettings/UI/FPS Dropdown")]
    public class FPSDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown targetElement;

        private void Reset()
        {
            targetElement = GetComponentInChildren<TMP_Dropdown>();
        }

        private void Start()
        {
            if (targetElement == null)
            {
                targetElement = GetComponentInChildren<TMP_Dropdown>();
                if (targetElement == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("[FPSDropdown] TextMeshPro Dropdown 컴포넌트를 찾을 수 없습니다.", gameObject);
#endif
                    return;
                }
            }

            CreateOptions(null);
            LocalizationSettings.SelectedLocaleChanged += CreateOptions;
        }

        void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= CreateOptions;
        }

        private void CreateOptions(Locale locale)
        {
            targetElement.ClearOptions();
            targetElement.onValueChanged.RemoveAllListeners();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (string fps in GameSettingsManager.FPSOptionNames)
            {
                string option = fps;
                if (fps == "NoLlimit")
                {
                    var currentLanguage = LocalizationSettings.SelectedLocale;
                    option = LocalizationSettings.StringDatabase.GetLocalizedString("SettingTable", fps, currentLanguage);
                }

                options.Add(new TMP_Dropdown.OptionData(option));
            }

            targetElement.AddOptions(options);
            targetElement.value = GameSettingsManager.MaxFPSIndex;
            targetElement.onValueChanged.AddListener(OnValueChange);
        }

        private void OnValueChange(int value)
        {
            GameSettingsManager.MaxFPSIndex = value;
        }
    }
}
