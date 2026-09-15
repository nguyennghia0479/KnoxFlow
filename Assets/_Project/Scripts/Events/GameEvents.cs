using System;

public static class GameEvents
{
    public static event Action<LevelSO> OnLevelLoaded;
    public static event Action<bool, int> OnLevelCompleted;
    public static event Action OnKnoxsConnected;

    public static void RaiseLevelLoaded(LevelSO levelSO)
    {
        OnLevelLoaded?.Invoke(levelSO);
    }

    public static void RaiseLevelCompleted(bool isPerfect, int moves)
    {
        OnLevelCompleted?.Invoke(isPerfect, moves);
    }

    public static void RaiseOnKnoxsConnected()
    {
        OnKnoxsConnected?.Invoke();
    }
}
