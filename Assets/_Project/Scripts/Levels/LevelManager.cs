using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelPackSO[] levelPackSOs;

    private LevelPackSO currentLevelPack;
    private LevelStageSO[] levelStageSOs;
    private LevelStageSO currentStage;
    private int currentStageIdx;
    private LevelSO currentLevel;
    private int currentLevelIdx;
    private bool isLastLevel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        UIEvents.OnPlayBtnClicked += HandlePlayButtonClicked;
        UIEvents.OnLevelPackSelected += HandleLevelPackSelected;
        UIEvents.OnNextLevelBtnClicked += HandleNextLevelButtonClicked;
        UIEvents.OnRetryBtnClicked += HandleRetryButtonClicked;
    }

    private void OnDisable()
    {
        UIEvents.OnPlayBtnClicked -= HandlePlayButtonClicked;
        UIEvents.OnLevelPackSelected -= HandleLevelPackSelected;
        UIEvents.OnNextLevelBtnClicked -= HandleNextLevelButtonClicked;
        UIEvents.OnRetryBtnClicked -= HandleRetryButtonClicked;
    }

    public void SetupLevelLoaded(LevelSO levelSO, LevelStageSO levelStageSO)
    {
        currentStage = levelStageSO;
        currentStageIdx = System.Array.IndexOf(levelStageSOs, levelStageSO);
        currentLevelIdx = System.Array.IndexOf(levelStageSOs[currentStageIdx].LevelSOs, levelSO);
        LoadLevel();
    }

    private void HandlePlayButtonClicked() => ClearSetup();

    private void HandleLevelPackSelected(LevelPackSO levelPackSO)
    {
        if (currentLevelPack == levelPackSO) // case for back to select level
        {
            ClearSetup();
            return;
        }

        currentLevelPack = levelPackSO;
        levelStageSOs = levelPackSO.LevelStageSOs;
    }

    private void ClearSetup()
    {
        currentStage = null;
        currentLevel = null;
        currentStageIdx = 0;
        currentLevelIdx = 0;
    }

    private void HandleNextLevelButtonClicked()
    {
        float waitTime = .25f;
        currentLevelIdx++;
        if (currentLevelIdx < currentStage.LevelSOs.Length)
            Invoke(nameof(LoadNextLevel), waitTime);
        else
            Invoke(nameof(LoadNextStage), waitTime);
    }

    private void HandleRetryButtonClicked()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        isLastLevel = currentLevelIdx == currentStage.LevelSOs.Length - 1;
        currentLevel = currentStage.LevelSOs[currentLevelIdx];
        GameEvents.RaiseLevelLoaded(currentLevel);
    }

    private void LoadNextLevel()
    {
        currentLevel = currentStage.LevelSOs[currentLevelIdx];
        UIEvents.RaiseLevelSelected(currentLevel, currentStage);
    }

    private void LoadNextStage()
    {
        currentStageIdx++;
        if (currentStageIdx >= levelStageSOs.Length)
            return;

        currentStage = levelStageSOs[currentStageIdx];
        currentLevelIdx = 0;
        currentLevel = currentStage.LevelSOs[currentLevelIdx];
        UIEvents.RaiseLevelSelected(currentLevel, currentStage);
    }

    public LevelStageSO GetNextLevelStageSO()
    {
        if (CanLoadNextStage)
            return levelStageSOs[(currentStageIdx + 1)];

        return null;
    }

    public LevelPackSO[] LevelPackSOs => levelPackSOs;
    public LevelPackSO LevelPackSO => currentLevelPack;
    public int LevelStageIndex => currentStageIdx;
    public bool IsLastLevel => isLastLevel;
    public bool CanLoadNextStage => (currentStageIdx + 1) < levelStageSOs.Length;
}
