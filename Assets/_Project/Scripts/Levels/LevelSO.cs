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

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [SerializeField] private string levelId;
    [SerializeField] private int levelNumber;
    [SerializeField] private int cols;
    [SerializeField] private int rows;
    [SerializeField] private KnoxInfo[] knoxInfos;
    [SerializeField] private Pack pack;

    private void OnValidate()
    {
        levelId = $"{pack}_{cols}x{rows}_{levelNumber}";
    }

    public void SaveLevelSO(int cols, int rows, KnoxInfo[] knoxInfos)
    {
        this.cols = cols;
        this.rows = rows;
        this.knoxInfos = knoxInfos;
    }

    public string LevelId => levelId;
    public string LevelName => $"{levelNumber} {cols}x{rows}";
    public int LevelNumber => levelNumber;
    public int Cols => cols;
    public int Rows => rows;
    public KnoxInfo[] Knoxs => knoxInfos;
}
