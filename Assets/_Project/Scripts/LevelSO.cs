using UnityEngine;

public enum KnoxColorType
{
    None, Red, Green, Blue, Yellow, Orange, Cyan
}

[System.Serializable]
public struct KnoxInfo
{
    public int coordX;
    public int coordY;
    public KnoxColorType knoxColorType;
}

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [SerializeField] private string levelId;
    [SerializeField] private int levelNumber;
    [SerializeField] private int cols;
    [SerializeField] private int rows;
    [SerializeField] private KnoxInfo[] knoxInfos;

    public int Cols => cols;
    public int Rows => rows;
    public KnoxInfo[] Knoxs => knoxInfos;
}
