using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] uiElements;
    [Space]

    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private SelectPackUI selectPackUI;
    [SerializeField] private SelectLevelUI selectLevelUI;
    [SerializeField] private MainGameUI mainGameUI;
    [SerializeField] private CompleteUI completeUI;
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private CreditsUI creditsUI;
    [SerializeField] private SettingsUI settingsUI;
    [SerializeField] private LoadingUI loadingUI;

    private void OnEnable()
    {
        UIEvents.OnPlayBtnClicked += HandlePlayButtonClicked;
        UIEvents.OnLevelPackSelected += HandleLevelPackSelected;
        UIEvents.OnLevelSelected += HandleLevelSelected;
        UIEvents.OnTutorialBtnClicked += HandleTutorialButtonClicked;
        UIEvents.OnCreditsBtnClicked += HandleCreditsButtonClicked;
        UIEvents.OnSettingsBtnClicked += HandleSettingButtonClicked;

        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        UIEvents.OnPlayBtnClicked -= HandlePlayButtonClicked;
        UIEvents.OnLevelPackSelected -= HandleLevelPackSelected;
        UIEvents.OnLevelSelected -= HandleLevelSelected;
        UIEvents.OnTutorialBtnClicked -= HandleTutorialButtonClicked;
        UIEvents.OnCreditsBtnClicked -= HandleCreditsButtonClicked;
        UIEvents.OnSettingsBtnClicked -= HandleSettingButtonClicked;

        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void Start()
    {
        SwitchToUI(mainMenuUI.gameObject);
        settingsUI.LoadSettings();
    }

    private void SwitchToUI(GameObject uiToEnable)
    {
        foreach (var uiElement in uiElements)
            uiElement.SetActive(false);

        uiToEnable.SetActive(true);
    }

    private void HandlePlayButtonClicked()
    {
        if (mainGameUI.gameObject.activeSelf)
        {
            loadingUI.DoLoading(() => SwitchToUI(selectPackUI.gameObject));
        }
        else
            SwitchToUI(selectPackUI.gameObject);
    }

    private void HandleLevelPackSelected(LevelPackSO levelPackSO)
    {
        if (mainGameUI.gameObject.activeSelf)
            loadingUI.DoLoading(() => LoadSelectLevelUI(levelPackSO));
        else
            LoadSelectLevelUI(levelPackSO);
    }

    private void LoadSelectLevelUI(LevelPackSO levelPackSO)
    {
        SwitchToUI(selectLevelUI.gameObject);
        selectLevelUI.SetupSelectLevelUI(levelPackSO);
    }

    private void HandleLevelSelected(LevelSO levelSO, LevelStageSO levelStageSO)
    {
        loadingUI.DoLoading(() => LoadMainGameUI(levelSO, levelStageSO));
    }

    private void LoadMainGameUI(LevelSO levelSO, LevelStageSO levelStageSO)
    {
        SwitchToUI(mainGameUI.gameObject);
        mainGameUI.SetupMainGameUI(levelSO, levelStageSO);
    }

    private void HandleTutorialButtonClicked()
    {
        SwitchToUI(tutorialUI.gameObject);
    }

    private void HandleCreditsButtonClicked()
    {
        SwitchToUI(creditsUI.gameObject);
    }

    private void HandleSettingButtonClicked()
    {
        settingsUI.gameObject.SetActive(true);
    }

    public void HandleCloseButtonClicked(GameObject uiToDisable)
    {
        uiToDisable.SetActive(false);
        UIEvents.RaiseButtonClicked();
    }

    public void HandleBackButtonClicked(GameObject uiToEnable)
    {
        UIEvents.RaiseButtonClicked();
        SwitchToUI(uiToEnable);
    }

    public void HandleMainMenuButtonClicked()
    {
        if (mainGameUI.gameObject.activeSelf)
        {
            loadingUI.DoLoading(() => SwitchToUI(mainMenuUI.gameObject));
            //UIEvents.RaiseMainMenuButtonClicked();
        }
        else
            SwitchToUI(mainMenuUI.gameObject);

        UIEvents.RaiseButtonClicked();
    }

    private void HandleLevelCompleted(bool isPerfect, int moves)
    {
        completeUI.SetupCompleteUI(isPerfect, moves);
        completeUI.gameObject.SetActive(true);
    }
}
