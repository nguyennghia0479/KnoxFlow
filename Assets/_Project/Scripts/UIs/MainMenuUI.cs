using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingsBtn;

    private void OnEnable()
    {
        if (playBtn)
            playBtn.onClick.AddListener(OnPlayButtonClicked);

        if (tutorialBtn)
            tutorialBtn.onClick.AddListener(OnTutorialButtonClicked);

        if (creditsBtn)
            creditsBtn.onClick.AddListener(OnCreditsButtonClicked);

        if (quitBtn)
            quitBtn.onClick.AddListener(OnQuitButtonClicked);

        if (settingsBtn)
            settingsBtn.onClick.AddListener(OnSettingsButtonClicked);
    }

    private void OnDisable()
    {
        if (playBtn)
            playBtn.onClick.RemoveListener(OnPlayButtonClicked);

        if (tutorialBtn)
            tutorialBtn.onClick.RemoveListener(OnTutorialButtonClicked);

        if (creditsBtn)
            creditsBtn.onClick.RemoveListener(OnCreditsButtonClicked);

        if (quitBtn)
            quitBtn.onClick.RemoveListener(OnQuitButtonClicked);

        if (settingsBtn)
            settingsBtn.onClick.RemoveListener(OnSettingsButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        UIEvents.RaisePlayButtonClicked();
    }

    private void OnTutorialButtonClicked()
    {
        UIEvents.RaiseTutorialButtonClicked();
    }

    private void OnCreditsButtonClicked()
    {
        UIEvents.RaiseCreditsButtonClicked();
    }

    private void OnQuitButtonClicked()
    {
        UIEvents.RaiseButtonClicked();
        Application.Quit();
    }

    private void OnSettingsButtonClicked()
    {
        UIEvents.RaiseSettingsButtonClicked();
    }
}
