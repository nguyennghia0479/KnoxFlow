using System;

public static class UIEvents
{
    public static event Action OnPlayBtnClicked;
    public static event Action<LevelPackSO> OnLevelPackSelected;
    public static event Action<LevelSO, LevelStageSO> OnLevelSelected;
    public static event Action OnUndoBtnClicked;
    public static event Action OnClearLevelBtnClicked;
    public static event Action OnNextLevelBtnClicked;
    public static event Action OnRetryBtnClicked;
    public static event Action OnMainMenuBtnClicked;
    public static event Action OnTutorialBtnClicked;
    public static event Action OnCreditsBtnClicked;
    public static event Action OnSettingsBtnClicked;
    public static event Action OnButtonClicked;

    public static void RaisePlayButtonClicked()
    {
        OnPlayBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseLevelPackSelected(LevelPackSO levelPackSO)
    {
        OnLevelPackSelected?.Invoke(levelPackSO);
        OnButtonClicked?.Invoke();
    }

    public static void RaiseLevelSelected(LevelSO levelSO, LevelStageSO levelStageSO)
    {
        OnLevelSelected?.Invoke(levelSO, levelStageSO);
        OnButtonClicked?.Invoke();
    }

    public static void RaiseUndoButtonClicked()
    {
        OnUndoBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseClearLevelButtonClicked()
    {
        OnClearLevelBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseNextLevelButtonClicked()
    {
        OnNextLevelBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseRetryButtonClicked()
    {
        OnRetryBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseMainMenuButtonClicked()
    {
        OnMainMenuBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseTutorialButtonClicked()
    {
        OnTutorialBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseCreditsButtonClicked()
    {
        OnCreditsBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseSettingsButtonClicked()
    {
        OnSettingsBtnClicked?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public static void RaiseButtonClicked()
    {
        OnButtonClicked?.Invoke();
    }
}
