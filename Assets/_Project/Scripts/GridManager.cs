using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum KnoxColorType
{
    None, Red, Green, Blue, Yellow, Orange
}

[System.Serializable]
public struct KnoxInfo
{
    public int coordX;
    public int coordY;
    public KnoxColorType knoxColorType;
}

[System.Serializable]
public struct UndoSnapshot
{
    public KnoxColorType knoxType;
    public List<CellSnapshot> path;
    public bool wasConnected;
}

public class GridManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Grid Layout Info")]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private RectTransform mainFrameRect;
    [SerializeField] private int cols;
    [SerializeField] private int rows;
    [Space]
    [SerializeField] private KnoxInfo[] knoxs;

    private GridLayoutGroup gridLayoutGroup;
    private float cellSize;
    private Cell[,] gridData;
    private GraphicRaycaster raycaster;
    private List<KnoxColorType> connectedKnoxs = new();
    private Dictionary<KnoxColorType, List<Cell>> knoxDict = new();
    private List<Cell> currentPath = new();
    private KnoxColorType currentKnox;
    private bool isDragging;
    private bool isCompleted;
    private bool canUndo;
    public List<UndoSnapshot> undoSnapshots;

    private void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        raycaster = GetComponentInParent<GraphicRaycaster>();
    }

    private void Start()
    {
        SetupGridLayoutGroup();
        GenerateGrid();
        GenerateKnoxs();
    }

    private void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
            ResetLevel();

        if (Keyboard.current.uKey.wasPressedThisFrame)
            UndoPaths();
    }

    private void SetupGridLayoutGroup()
    {
        Canvas.ForceUpdateCanvases();
        float mainFrameWidth = mainFrameRect.rect.width;
        float mainFrameHeight = mainFrameRect.rect.height;
        cellSize = Mathf.Min(mainFrameWidth / cols, mainFrameHeight / rows);

        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = cols;
        gridLayoutGroup.cellSize = new(cellSize, cellSize);
    }

    private void GenerateGrid()
    {
        gridData = new Cell[cols, rows];
        int gridSize = cols * rows;
        for (int i = 0; i < gridSize; i++)
        {
            int coordX = i % cols;
            int coordY = i / cols;
            Cell newCell = Instantiate(cellPrefab, transform);
            newCell.SetupCell(coordX, coordY, cellSize);
            gridData[coordX, coordY] = newCell;
        }
    }

    private void GenerateKnoxs()
    {
        for (int i = 0; i < knoxs.Length; i++)
        {
            KnoxInfo knox = knoxs[i];
            Cell cell = gridData[knox.coordX, knox.coordY];
            if (cell != null)
                cell.SetupKnox(knox.knoxColorType);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCompleted) return;

        Cell startCell = GetCellUnderPointer(eventData);
        if (startCell == null || (startCell.CellType != CellType.Knox && startCell.CellType != CellType.TempKnox && startCell.CellType != CellType.Pipe))
            return;

        isDragging = true;
        KnoxColorType newKnox = startCell.CellType == CellType.Pipe ? startCell.IsOccupiedCell : startCell.KnoxColorType;
        if (!knoxDict.ContainsKey(newKnox))
            knoxDict[newKnox] = new List<Cell>();

        undoSnapshots.Clear();
        CaptureUndoSnapshot(newKnox, knoxDict[newKnox]);
        currentKnox = newKnox;
        currentPath = knoxDict[currentKnox];

        if (startCell.CellType == CellType.Knox)
        {
            ClearCurrentPath(currentPath);
            startCell.IsOccupiedCell = currentKnox;
            currentPath.Add(startCell);
        }
        else if (startCell.CellType == CellType.TempKnox)
        {
            startCell.ClearTempKnox();
            startCell.EnableTempKnoxUI(true, currentKnox);
        }
        else if (startCell.CellType == CellType.Pipe)
        {
            RemoveCell(startCell, currentPath, currentKnox);
        }
    }

    private void RemoveCell(Cell startCell, List<Cell> currentPath, KnoxColorType currentKnox)
    {
        int index = currentPath.IndexOf(startCell);
        for (int i = currentPath.Count - 1; i > index; i--)
            ClearCell(currentPath[i]);

        int removeCount = currentPath.Count - (index + 1);
        if (removeCount > 0)
            currentPath.RemoveRange(index + 1, removeCount);

        Cell previousCell = currentPath[index - 1];
        startCell.ClearAllConnections();
        int coordX = startCell.CoordX - previousCell.CoordX;
        int coordY = startCell.CoordY - previousCell.CoordY;

        if (coordX > 0)
            startCell.SetConnection(Direction.Left, true, currentKnox);
        else if (coordX < 0)
            startCell.SetConnection(Direction.Right, true, currentKnox);

        if (coordY > 0)
            startCell.SetConnection(Direction.Up, true, currentKnox);
        else if (coordY < 0)
            startCell.SetConnection(Direction.Down, true, currentKnox);

        if (connectedKnoxs.Contains(currentKnox))
            connectedKnoxs.Remove(currentKnox);

        startCell.EnableTempKnoxUI(true, currentKnox);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || currentPath == null || currentPath.Count <= 0)
            return;

        Cell currentCell = GetCellUnderPointer(eventData);
        if (currentCell == null) return;

        Cell lastCell = currentPath[^1];
        if (lastCell == currentCell) return;
       
        if (!currentPath.Contains(currentCell) && IsAdjacent(lastCell, currentCell))
        {
            bool isCellFree = currentCell.CellType == CellType.None && currentCell.IsOccupiedCell == KnoxColorType.None;
            bool isValidKnox = currentCell.CellType == CellType.Knox && currentCell.KnoxColorType == currentKnox;

            if (!isCellFree && !isValidKnox)
            {
                if (currentCell.CellType == CellType.Knox) return;

                KnoxColorType affectedKnox = currentCell.IsOccupiedCell;
                List<Cell> affectedPath = knoxDict[currentCell.IsOccupiedCell];
                CaptureUndoSnapshot(affectedKnox, affectedPath);

                int previousIndex = affectedPath.IndexOf(currentCell) - 1;
                if (previousIndex > 0)
                {
                    Cell affectedCell = affectedPath[previousIndex];
                    RemoveCell(affectedCell, affectedPath, currentCell.IsOccupiedCell);
                    affectedCell.SetupTempKnox(affectedKnox);
                }
                else
                    ClearCurrentPath(affectedPath);
            }
            else
            {
                Direction dirFromLastToCurrent = GetDirection(lastCell, currentCell);
                Direction dirFromCurrentToLast = GetOppositeDirection(dirFromLastToCurrent);

                lastCell.EnableTempKnoxUI(false, KnoxColorType.None);
                currentCell.EnableTempKnoxUI(true, currentKnox);
                lastCell.SetConnection(dirFromLastToCurrent, true, currentKnox);
                currentCell.SetConnection(dirFromCurrentToLast, true, currentKnox);

                currentCell.IsOccupiedCell = currentKnox;
                currentPath.Add(currentCell);
            }

            if (currentCell.CellType == CellType.Knox && currentCell.KnoxColorType == currentKnox && currentCell != currentPath[0])
            {
                canUndo = true;
                isDragging = false;
                connectedKnoxs.Add(currentKnox);
                if (connectedKnoxs.Count == knoxs.Length / 2)
                {
                    isCompleted = true;
                    Debug.Log("Completed Level");
                }
            }
        }
        else if (currentPath.Count > 1 && currentCell == currentPath[^2])
        {
            Direction dirFromLastToCurrent = GetDirection(lastCell, currentCell);
            Direction dirFromCurrentToLast = GetOppositeDirection(dirFromLastToCurrent);

            lastCell.EnableTempKnoxUI(false, KnoxColorType.None);
            currentCell.EnableTempKnoxUI(true, currentKnox);
            lastCell.SetConnection(dirFromLastToCurrent, false, KnoxColorType.None);
            currentCell.SetConnection(dirFromCurrentToLast, false, KnoxColorType.None);

            if (lastCell.CellType == CellType.None)
                lastCell.IsOccupiedCell = KnoxColorType.None;

            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        canUndo = CanUndo();
        isDragging = false;

        if (currentPath == null || currentPath.Count <= 1)
        {
            ClearCurrentPath(currentPath);
            return;
        }

        Cell lastCell = currentPath[^1];
        if (lastCell.CellType != CellType.Knox && lastCell.IsOccupiedCell != KnoxColorType.None)
            lastCell.SetupTempKnox(currentKnox);
    }

    private Cell GetCellUnderPointer(PointerEventData eventData)
    {
        List<RaycastResult> raycasts = new();
        raycaster.Raycast(eventData, raycasts);

        foreach (var raycast in raycasts)
        {
            Cell cell = raycast.gameObject.GetComponentInParent<Cell>();
            if (cell != null)
                return cell;
        }

        return null;
    }

    private bool IsAdjacent(Cell lastCell, Cell currentCell)
    {
        int coordX = Mathf.Abs(lastCell.CoordX - currentCell.CoordX);
        int coordY = Mathf.Abs(lastCell.CoordY - currentCell.CoordY);

        return (coordX == 1 && coordY == 0) || (coordX == 0 && coordY == 1);
    }

    private Direction GetDirection(Cell lastCell, Cell currentCell)
    {
        if (currentCell.CoordX < lastCell.CoordX) return Direction.Left;
        if (currentCell.CoordX > lastCell.CoordX) return Direction.Right;
        if (currentCell.CoordY > lastCell.CoordY) return Direction.Down;
        if (currentCell.CoordY < lastCell.CoordY) return Direction.Up;

        return Direction.None;
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.None,
        };
    }

    private void ClearCurrentPath(List<Cell> currentPath)
    {
        if (currentPath == null || currentPath.Count <= 0) return;

        KnoxColorType currentKnox = currentPath[0].KnoxColorType;
        foreach (var cell in currentPath)
            ClearCell(cell);

        currentPath.Clear();
        if (connectedKnoxs.Contains(currentKnox))
            connectedKnoxs.Remove(currentKnox);
    }

    private void ClearCell(Cell cell)
    {
        if (cell == null) return;

        cell.ClearAllConnections();
        cell.IsOccupiedCell = KnoxColorType.None;
        if (cell.CellType == CellType.TempKnox)
            cell.ClearTempKnox();
    }

    private void ResetLevel()
    {
        if (isCompleted) return;

        isCompleted = false;
        currentKnox = KnoxColorType.None;
        connectedKnoxs.Clear();
        foreach (var knox in knoxDict)
        {
            currentPath = knox.Value;
            ClearCurrentPath(currentPath);
        }
    }

    private bool CanUndo()
    {
        foreach (var undoSnapshot in undoSnapshots)
        {
            List<Cell> currentPath = knoxDict[undoSnapshot.knoxType];

            if (currentPath.Count != undoSnapshot.path.Count)
                return true;

            for (int i = 0; i < undoSnapshot.path.Count; i++)
            {
                if (currentPath[i] != undoSnapshot.path[i].cell)
                    return true;
            }

            bool hasConnected = connectedKnoxs.Contains(undoSnapshot.knoxType);
            if (hasConnected != undoSnapshot.wasConnected)
                return true;
        }

        return false;
    }

    private void CaptureUndoSnapshot(KnoxColorType currentKnox, List<Cell> currentPath)
    {
        if (currentPath == null) return;

        if (undoSnapshots.Any(x => x.knoxType == currentKnox)) return;

        UndoSnapshot undoSnapshot = new()
        {
            knoxType = currentKnox,
            wasConnected = connectedKnoxs.Contains(currentKnox),
            path = CreateCellSnapshots(currentPath)
        };

        undoSnapshots.Add(undoSnapshot);
    }

    private List<CellSnapshot> CreateCellSnapshots(List<Cell> path)
    {
        List<CellSnapshot> cellSnapshotPath = new();
        foreach (var cell in path)
        {
            CellSnapshot cellSnapshot = new()
            {
                cell = cell,
                cellType = cell.CellType,
                isOccupiedCell = cell.IsOccupiedCell,
                isLineUpActive = cell.IsLineUpActive,
                isLineDownActive = cell.IsLineDownActve,
                isLineLeftActive = cell.IsLineLeftActive,
                isLineRightActive = cell.IsLineRightActive
            };
            cellSnapshotPath.Add(cellSnapshot);
        }

        return cellSnapshotPath;
    }

    private void UndoPaths()
    {
        if (!canUndo || isCompleted) return;

        foreach (var undoSnapshot in undoSnapshots)
            UndoPath(undoSnapshot);

        undoSnapshots.Clear();
        canUndo = false;
    }

    private void UndoPath(UndoSnapshot undoSnapshot)
    {
        if (!canUndo || isCompleted) return;
        
        List<Cell> currentPath = knoxDict[undoSnapshot.knoxType];
        if (undoSnapshot.path.Count == 0)
        {
            ClearCurrentPath(currentPath);
            return;
        }

        for (int i = currentPath.Count - 1; i >= 0; i--)
            ClearCell(currentPath[i]);
        currentPath.Clear();

        for (int i = 0; i < undoSnapshot.path.Count; i++)
        {
            CellSnapshot cellSnapShot = undoSnapshot.path[i];
            Cell cell = cellSnapShot.cell;
            cell.RestoreCell(cellSnapShot);
            currentPath.Add(cell);
        }

        if (undoSnapshot.wasConnected)
            connectedKnoxs.Add(undoSnapshot.knoxType);
        else
            connectedKnoxs.Remove(undoSnapshot.knoxType);
    }
}
