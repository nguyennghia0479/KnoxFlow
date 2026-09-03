using UnityEngine;
using UnityEngine.UI;

public enum CellType
{
    None, Knox, TempKnox, Pipe
}

public enum Direction
{
    None, Up, Down, Left, Right
}

[System.Serializable]
public struct CellSnapshot
{
    public Cell cell;
    public CellType cellType;
    public KnoxColorType isOccupiedCell;
    public bool isLineUpActive;
    public bool isLineDownActive;
    public bool isLineLeftActive;
    public bool isLineRightActive;
}

public class Cell : MonoBehaviour
{
    [Header("Image Elements")]
    [SerializeField] private Image knoxImg;
    [SerializeField] private Image tempKnoxImg;
    [SerializeField] private Image lineUpImg;
    [SerializeField] private Image lineDownImg;
    [SerializeField] private Image lineLeftImg;
    [SerializeField] private Image lineRightImg;
    [SerializeField] private Image centerFillImg;

    [Header("RectTransform Elements")]
    [SerializeField] private RectTransform tempKnoxRect;
    [SerializeField] private RectTransform lineUpRect;
    [SerializeField] private RectTransform lineDownRect;
    [SerializeField] private RectTransform lineLeftRect;
    [SerializeField] private RectTransform lineRightRect;
    [SerializeField] private RectTransform centerFillRect;

    private int coordX;
    private int coordY;
    private KnoxColorType knoxColorType;
    private CellType cellType;

    public void SetupCell(int coordX, int coordY, float cellSize)
    {
        this.coordX = coordX;
        this.coordY = coordY;
        gameObject.name = $"Cell_{coordX}_{coordY}";
        SetupSize(cellSize);
    }

    private void SetupSize(float cellSize)
    {
        float lineThickness = cellSize * .25f;
        float tempKnoxSize = lineThickness * 2;

        tempKnoxRect.sizeDelta = new Vector2(tempKnoxSize, tempKnoxSize);
        lineUpRect.sizeDelta = new Vector2(lineThickness, 0);
        lineDownRect.sizeDelta = new Vector2(lineThickness, 0);
        lineLeftRect.sizeDelta = new Vector2(0, lineThickness);
        lineRightRect.sizeDelta = new Vector2(0, lineThickness);
        centerFillRect.sizeDelta = new Vector2(lineThickness, lineThickness);
    }

    public void SetupKnox(KnoxColorType knoxColorType)
    {
        this.knoxColorType = knoxColorType;
        knoxImg.color = GetColorByType(knoxColorType);
        knoxImg.gameObject.SetActive(true);
        cellType = CellType.Knox;
    }

    public void SetupTempKnox(KnoxColorType knoxColorType)
    {
        this.knoxColorType = knoxColorType;
        tempKnoxImg.color = GetColorByType(knoxColorType);
        tempKnoxImg.gameObject.SetActive(true);
        cellType = CellType.TempKnox;
    }

    public void ClearTempKnox()
    {
        tempKnoxImg.gameObject.SetActive(false);
        cellType = CellType.None;
        knoxColorType = KnoxColorType.None;
    }

    public void SetConnection(Direction direction, bool active, KnoxColorType knoxColorType)
    {
        Image targetLine = direction switch
        {
            Direction.Up => lineUpImg,
            Direction.Down => lineDownImg,
            Direction.Left => lineLeftImg,
            Direction.Right => lineRightImg,
            _ => null
        };

        if (targetLine != null)
        {
            Color color = GetColorByType(knoxColorType);
            targetLine.gameObject.SetActive(active);
            if (active && color != null)
                targetLine.color = color;

            bool showCenter = IsCorner();
            centerFillImg.gameObject.SetActive(showCenter);
            if (showCenter && color != null)
                centerFillImg.color = color;

            if (cellType != CellType.Knox && cellType != CellType.TempKnox)
                cellType = active ? CellType.Pipe : CellType.None;
        }
    }

    public void ClearAllConnections()
    {
        lineUpImg.gameObject.SetActive(false);
        lineDownImg.gameObject.SetActive(false);
        lineLeftImg.gameObject.SetActive(false);
        lineRightImg.gameObject.SetActive(false);
        centerFillImg.gameObject.SetActive(false);

        if (cellType == CellType.Pipe)
            cellType = CellType.None;
    }

    public void RestoreCell(CellSnapshot cellSnapshot)
    {
        IsOccupiedCell = cellSnapshot.isOccupiedCell;
        SetConnection(Direction.Up, cellSnapshot.isLineUpActive, cellSnapshot.isLineUpActive ? cellSnapshot.isOccupiedCell : KnoxColorType.None);
        SetConnection(Direction.Down, cellSnapshot.isLineDownActive, cellSnapshot.isLineDownActive ? cellSnapshot.isOccupiedCell : KnoxColorType.None);
        SetConnection(Direction.Left, cellSnapshot.isLineLeftActive, cellSnapshot.isLineLeftActive ? cellSnapshot.isOccupiedCell : KnoxColorType.None);
        SetConnection(Direction.Right, cellSnapshot.isLineRightActive, cellSnapshot.isLineRightActive ? cellSnapshot.isOccupiedCell : KnoxColorType.None);
        if (centerFillImg.gameObject.activeSelf)
        {
            Color color = GetColorByType(cellSnapshot.isOccupiedCell);
            centerFillImg.color = color;
        }

        cellType = cellSnapshot.cellType;
        if (cellType == CellType.TempKnox)
            SetupTempKnox(cellSnapshot.isOccupiedCell);
    }

    public void EnableTempKnoxUI(bool enabled, KnoxColorType knoxColorType)
    {
        tempKnoxImg.color = GetColorByType(knoxColorType);
        tempKnoxImg.gameObject.SetActive(enabled);
    }

    private Color GetColorByType(KnoxColorType knoxColorType)
    {
        return knoxColorType switch
        {
            KnoxColorType.Red => Color.red,
            KnoxColorType.Green => Color.green,
            KnoxColorType.Blue => Color.blue,
            KnoxColorType.Yellow => Color.yellow,
            KnoxColorType.Orange => Color.orange,
            _ => Color.black,
        };
    }

    private bool IsCorner()
    {
        bool isVerticalActive = lineUpImg.gameObject.activeSelf || lineDownImg.gameObject.activeSelf;
        bool isHorizontalActive = lineLeftImg.gameObject.activeSelf || lineRightImg.gameObject.activeSelf;

        return isVerticalActive && isHorizontalActive;
    }

    public int CoordX => coordX;
    public int CoordY => coordY;
    public KnoxColorType KnoxColorType => knoxColorType;
    public CellType CellType => cellType;
    public KnoxColorType IsOccupiedCell { get; set; } = KnoxColorType.None;
    public bool IsLineUpActive => lineUpImg.gameObject.activeSelf;
    public bool IsLineDownActve => lineDownImg.gameObject.activeSelf;
    public bool IsLineLeftActive => lineLeftImg.gameObject.activeSelf;
    public bool IsLineRightActive => lineRightImg.gameObject.activeSelf;
}
