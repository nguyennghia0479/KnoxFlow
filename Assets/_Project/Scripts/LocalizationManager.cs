using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private void OnEnable()
    {
        UIEvents.OnDropdownChanged += HandleDropdownChanged;
    }

    private void OnDisable()
    {
        UIEvents.OnDropdownChanged -= HandleDropdownChanged;
    }

    private void HandleDropdownChanged(string localeCode)
    {
        StartCoroutine(ChangeLanguageRoutine(localeCode));
    }

    private IEnumerator ChangeLanguageRoutine(string localeCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
    }
}
