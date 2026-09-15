using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("SFX Setting")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParam;
    [SerializeField] private float decibelMultiplier = 40f;

    [Header("Language Setting")]
    [SerializeField] private TMP_Dropdown langDropdown;

    private const float MIN_SLIDER_VALUE = .0001f;
    private const float DEFAULT_SFX_VALUE = .5f;
    private const int DEFAULT_LOCALE_VALUE = 0;
    private const string LANG_EN = "en";
    private const string LANG_VI_VN = "vi-VN";
    private const string LOCALE = "Locale";

    private void OnEnable()
    {
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        langDropdown.onValueChanged.AddListener(OnDropdownChanged);

        LoadSettings();
    }

    private void OnDisable()
    {
        sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
        langDropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        SaveSettings();
    }

    private void OnSFXChanged(float sliderValue)
    {
        float clampedValue = Mathf.Max(sliderValue, MIN_SLIDER_VALUE);
        float decibel = Mathf.Log10(clampedValue) * decibelMultiplier;
        audioMixer.SetFloat(sfxParam, decibel);
    }

    private void OnDropdownChanged(int dropdownValue)
    {
        string selectedLocale = dropdownValue == 0 ? LANG_EN : LANG_VI_VN;
        UIEvents.RaiseDropdownChanged(selectedLocale);
    }

    private void SaveSettings()
    {
        SaveManager.SaveSFX(sfxParam, sfxSlider.value);
        SaveManager.SaveLocale(LOCALE, langDropdown.value);
    }

    public void LoadSettings()
    {
        float loadSFXVal = SaveManager.LoadSFX(sfxParam, DEFAULT_SFX_VALUE);
        sfxSlider.value = loadSFXVal;
        OnSFXChanged(loadSFXVal);

        int loadLocaleVal = SaveManager.LoadLocale(LOCALE, DEFAULT_LOCALE_VALUE);
        langDropdown.value = loadLocaleVal;
        OnDropdownChanged(loadLocaleVal);
    }

}
