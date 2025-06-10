using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace FrameWork.GameSettings
{
    [AddComponentMenu("GameSettings/UI/Localization Dropdown")]
    public class LocalizationDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown targetElement;

        private void Reset()
        {
            targetElement = GetComponentInChildren<TMP_Dropdown>();
        }

        private void Awake()
        {
            if (targetElement == null)
            {
                targetElement = GetComponentInChildren<TMP_Dropdown>();
                if (targetElement == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("[LocalizationDropdown] TextMeshPro Dropdown ÄÄÆ÷³ÍÆ®¸¦ Ã£À» ¼ö ¾ø½À´Ï´Ù.", gameObject);
#endif
                    return;
                }
            }

            targetElement.ClearOptions();
            targetElement.onValueChanged.RemoveAllListeners();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                string option;
                
                switch (locale.Identifier.Code)
                {
                    case "ko":
                        option = "ÇÑ±¹¾î";
                        break;
                    case "en":
                        option = "English";
                        break;
                    case "ja":
                        option = "ìíÜâåÞ";
                        break;
                    case "zh":
                        option = "ñéÙþ";
                        break;
                    default:
                        option = string.Empty;
                        break;
                }
                options.Add(new TMP_Dropdown.OptionData(option));
            }

            targetElement.AddOptions(options);
            targetElement.value = GetSelectedLocaleIndex();
            targetElement.onValueChanged.AddListener(OnValueChange);
        }

        private int GetSelectedLocaleIndex()
        {
            var selectedLocale = LocalizationSettings.SelectedLocale;
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (LocalizationSettings.AvailableLocales.Locales[i] == selectedLocale)
                {
                    return i;
                }
            }
            return 0;
        }

        private void OnValueChange(int idx)
        {
            StartCoroutine(ChangeLanguage(idx));
        }

        IEnumerator ChangeLanguage(int idx)
        {
            targetElement.interactable = false;

            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[idx];

            targetElement.interactable = true;
        }
    }
}
