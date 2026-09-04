using System;

public static class GameEvents
{
    public static event Action<LevelSO> OnLevelLoaded;
    public static event Action OnLevelCompleted;

    public static void RaiseLevelLoaded(LevelSO levelSO)
    {
        OnLevelLoaded?.Invoke(levelSO);
    }

    public static void RaiseLevelCompleted()
    {
        OnLevelCompleted?.Invoke();
    }
}
