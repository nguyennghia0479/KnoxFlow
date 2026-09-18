using UnityEngine;

public enum KnoxColorType
{
    None, Red, Green, Blue, Yellow, Orange, Cyan, Pink, Brown, Purple, White
}

[System.Serializable]
public struct KnoxInfo
{
    public int coordX;
    public int coordY;
    public KnoxColorType knoxColorType;
}

[System.Serializable]
public struct Coordinate
{
    public int coordX;
    public int coordY;
}

[System.Serializable]
public struct KnoxHint
{
    public KnoxColorType knoxColorType;
    public Coordinate[] coordinates;
}

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [SerializeField] private string levelId;
    [SerializeField] private int levelNumber;
    [SerializeField] private int cols;
    [SerializeField] private int rows;
    [SerializeField] private KnoxInfo[] knoxInfos;
    [SerializeField] private Pack pack;
    [SerializeField] private KnoxHint[] knoxHints;

    private void OnValidate()
    {
        levelId = $"{pack}_{cols}x{rows}_{levelNumber}";
    }

    public void SaveLevelSO(int cols, int rows, KnoxInfo[] knoxInfos, KnoxHint[] knoxHints)
    {
        this.cols = cols;
        this.rows = rows;
        this.knoxInfos = knoxInfos;
        this.knoxHints = knoxHints;
    }

    public string LevelId => levelId;
    public string LevelName => $"{levelNumber} {cols}x{rows}";
    public int LevelNumber => levelNumber;
    public int Cols => cols;
    public int Rows => rows;
    public KnoxInfo[] Knoxs => knoxInfos;
    public KnoxHint[] Hints => knoxHints;
}
