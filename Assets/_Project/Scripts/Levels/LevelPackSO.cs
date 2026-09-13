using UnityEngine;

public enum Pack
{
    Classic, Bonus
}

[CreateAssetMenu(fileName = "Pack", menuName = "Scriptable Objects/LevelPackSO")]
public class LevelPackSO : ScriptableObject
{
    [SerializeField] private string packName;
    [SerializeField] private LevelStageSO[] levelStageSOs;

    public string PackName => packName;
    public LevelStageSO[] LevelStageSOs => levelStageSOs;

    public int GetLevelAmountInPack()
    {
        int levelAmount = 0;
        foreach (var levelStage in levelStageSOs)
        {
            levelAmount += levelStage.LevelSOs.Length;
        }

        return levelAmount;
    }
}
