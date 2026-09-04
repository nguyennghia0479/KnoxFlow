using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelDatabaseSO[] stageLevels;

    private LevelDatabaseSO currentStage;
    private int currentStageIdx;
    private LevelSO currentLevel;
    private int currentLevelIdx;

    private void OnEnable()
    {
        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void Start()
    {
        currentStage = stageLevels[currentStageIdx];
        LoadLevel(); 
    }

    private void HandleLevelCompleted()
    {
        currentLevelIdx++;
        if (currentLevelIdx < currentStage.LevelSO.Length)
            Invoke(nameof(LoadLevel), 1);
        else
            Invoke(nameof(LoadNextStage), 1);
    }

    private void LoadLevel()
    {
        currentLevel = currentStage.LevelSO[currentLevelIdx];
        GameEvents.RaiseLevelLoaded(currentLevel);
    }

    private void LoadNextStage()
    {
        currentStageIdx++;
        if (currentStageIdx >= stageLevels.Length)
        {
            Debug.Log("You have completed all level stage!");
            return;
        }

        currentStage = stageLevels[currentStageIdx];
        currentLevelIdx = 0;
        LoadLevel();
    }
}
