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
}
