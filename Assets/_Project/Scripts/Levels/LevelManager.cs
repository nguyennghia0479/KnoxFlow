using System.Collections.Generic;
using UnityEngine;

public class LevelDTO
{
    public bool isCompleted;
    public bool isPerfect;
    public int best;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelPackSO[] levelPackSOs;

    private Dictionary<string, LevelDTO> levelCache = new();
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

        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        UIEvents.OnPlayBtnClicked -= HandlePlayButtonClicked;
        UIEvents.OnLevelPackSelected -= HandleLevelPackSelected;
        UIEvents.OnNextLevelBtnClicked -= HandleNextLevelButtonClicked;
        UIEvents.OnRetryBtnClicked -= HandleRetryButtonClicked;

        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
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
            //ClearSetup();
            return;
        }

        currentLevelPack = levelPackSO;
        levelStageSOs = levelPackSO.LevelStageSOs;
    }

    public int GetLevelCompletedInPack(LevelPackSO levelPackSO)
    {
        int levelCompletedAmount = 0;
        foreach (var stage in levelPackSO.LevelStageSOs)
        {
            foreach (var level in stage.LevelSOs)
            {
                LevelDTO leveDTO = GetLevelDTO(level.LevelId);
                if (leveDTO.isCompleted || leveDTO.isPerfect)
                    levelCompletedAmount++;
            }
        }

        return levelCompletedAmount;
    }

    public LevelDTO GetLevelDTO(string levelId)
    {
        if (levelCache.TryGetValue(levelId, out LevelDTO levelDTO))
            return levelDTO;
        
        levelDTO = SaveManager.LoadLevel(levelId);
        levelDTO ??= new();
        levelCache.Add(levelId, levelDTO);
        return levelDTO;
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

    private void HandleLevelCompleted(bool isPerfect, int moves)
    {
        if (!levelCache.TryGetValue(currentLevel.LevelId, out LevelDTO levelDTO))
        {
            Debug.LogError("Not found level id " + currentLevel.LevelId);
            return;
        }

        levelDTO.isCompleted = true;
        levelDTO.isPerfect = levelDTO.isPerfect || isPerfect;
        levelDTO.best = (levelDTO.best <= 0 || moves < levelDTO.best) ? moves : levelDTO.best;
        SaveManager.SaveLevel(currentLevel.LevelId, levelDTO);
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
