using UnityEngine;

[CreateAssetMenu(fileName = "Level Database", menuName = "Scriptable Objects/LevelDatabaseSO")]
public class LevelDatabaseSO : ScriptableObject
{
    [SerializeField] private string stageName;
    [SerializeField] private LevelSO[] levelSOs;

    public string StageName => stageName;
    public LevelSO[] LevelSO => levelSOs;
}
