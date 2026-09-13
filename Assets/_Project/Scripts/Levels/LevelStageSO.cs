using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Objects/LevelStageSO")]
public class LevelStageSO : ScriptableObject
{
    [SerializeField] private string stageName;
    [SerializeField] private LevelSO[] levelSOs;

    public string StageName => stageName;
    public LevelSO[] LevelSOs => levelSOs;
}
