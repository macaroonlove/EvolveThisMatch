using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace FrameWork.GameSettings
{
    public class LocalizationRefresh : MonoBehaviour
    {
        private void Start()
        {
            LocalizationSettings.SelectedLocaleChanged += OnChangeLanguage;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnChangeLanguage;
        }

        private void OnChangeLanguage(Locale locale)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}
