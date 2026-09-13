using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct UndoSnapshot
{
    public KnoxColorType knoxType;
    public List<CellSnapshot> path;
    public bool wasConnected;
}

public class GridManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static GridManager Instance { get; private set; }

    public event Action<int> KnoxsConnected;
    public event Action<int> Moved;
    public event Action<bool> UndoStateChanged;

    [Header("Grid Layout Info")]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private RectTransform mainFrameRect;

    private int cols;
    private int rows;
    private KnoxInfo[] knoxs;
    private GridLayoutGroup gridLayoutGroup;
    private float cellSize;
    private Cell[,] gridData;
    private GraphicRaycaster raycaster;
    private bool isDragging;
    private Dictionary<KnoxColorType, List<Cell>> knoxDict = new();
    private List<Cell> currentPath = new();
    private KnoxColorType currentKnox;
    private int moves;
    private List<KnoxColorType> connectedKnoxs = new();
    private int knoxsToConnect;
    private bool isCompleted;
    private bool canUndo;
    private List<UndoSnapshot> undoSnapshots = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        raycaster = GetComponentInParent<GraphicRaycaster>();
    }

    private void OnEnable()
    {
        GameEvents.OnLevelLoaded += HandleLevelLoaded;
        UIEvents.OnUndoBtnClicked += HandleUndoButtonClicked;
        UIEvents.OnClearLevelBtnClicked += HandleClearLevelButtonClicked;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelLoaded -= HandleLevelLoaded;
        UIEvents.OnUndoBtnClicked -= HandleUndoButtonClicked;
        UIEvents.OnClearLevelBtnClicked -= HandleClearLevelButtonClicked;
    }

    private void HandleLevelLoaded(LevelSO leveSO)
    {
        cols = leveSO.Cols;
        rows = leveSO.Rows;
        knoxs = leveSO.Knoxs;

        ResetGrid();
        SetupGridLayoutGroup();
        GenerateGrid();
        GenerateKnoxs();
    }

    private void HandleUndoButtonClicked() => UndoPaths();
    private void HandleClearLevelButtonClicked() => ClearLevel();
    
    private void ResetGrid()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        gridData = null;
        currentKnox = KnoxColorType.None;
        connectedKnoxs.Clear();
        knoxDict.Clear();
        currentPath.Clear();
        undoSnapshots.Clear();
        isDragging = false;
        moves = 0;
        isCompleted = false;
        canUndo = false;
        KnoxsConnected?.Invoke(connectedKnoxs.Count);
        Moved?.Invoke(moves);
        UndoStateChanged?.Invoke(canUndo);
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

        knoxsToConnect = knoxs.Length / 2;
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
            RemoveAffectedCells(startCell, currentPath, currentKnox);
    }

    private void RemoveAffectedCells(Cell startCell, List<Cell> currentPath, KnoxColorType currentKnox)
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

        RemoveKnoxsIfNeeded(currentKnox);
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
            if (!OnDragForward(currentCell, lastCell)) return;

            CheckKnoxsConnected(currentCell);
        }
        else if (currentPath.Count > 1 && currentCell == currentPath[^2])
            OnDragBackward(currentCell, lastCell);
    }

    private bool OnDragForward(Cell currentCell, Cell lastCell)
    {
        bool isCellFree = currentCell.CellType == CellType.None && currentCell.IsOccupiedCell == KnoxColorType.None;
        bool isValidKnox = currentCell.CellType == CellType.Knox && currentCell.KnoxColorType == currentKnox;

        if (!isCellFree && !isValidKnox)
        {
            if (currentCell.CellType == CellType.Knox) return false;

            KnoxColorType affectedKnox = currentCell.IsOccupiedCell;
            List<Cell> affectedPath = knoxDict[currentCell.IsOccupiedCell];
            CaptureUndoSnapshot(affectedKnox, affectedPath);

            int previousIndex = affectedPath.IndexOf(currentCell) - 1;
            if (previousIndex > 0)
            {
                Cell affectedCell = affectedPath[previousIndex];
                RemoveAffectedCells(affectedCell, affectedPath, currentCell.IsOccupiedCell);
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

        return true;
    }

    private void OnDragBackward(Cell currentCell, Cell lastCell)
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

    private void CheckKnoxsConnected(Cell currentCell)
    {
        if (currentCell.CellType == CellType.Knox && currentCell.KnoxColorType == currentKnox && currentCell != currentPath[0])
        {
            canUndo = true;
            isDragging = false;
            connectedKnoxs.Add(currentKnox);
            moves++;
            KnoxsConnected?.Invoke(connectedKnoxs.Count);
            Moved?.Invoke(moves);
            UndoStateChanged?.Invoke(canUndo);
            CheckLevelCompleted();
        }
    }

    private void CheckLevelCompleted()
    {
        if (connectedKnoxs.Count == knoxsToConnect)
        {
            isCompleted = true;
            canUndo = false;
            UndoStateChanged?.Invoke(canUndo);
            float waitTime = .5f;
            Invoke(nameof(LevelComplete), waitTime);
        }
    }

    private void LevelComplete()
    {
        bool isPerfect = knoxsToConnect == moves;
        GameEvents.RaiseLevelCompleted(isPerfect, moves);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (currentPath == null || currentPath.Count <= 1)
        {
            ClearCurrentPath(currentPath);
            return;
        }

        canUndo = CanUseUndo();
        isDragging = false;
        Cell lastCell = currentPath[^1];
        if (lastCell.CellType != CellType.Knox && lastCell.IsOccupiedCell != KnoxColorType.None)
            lastCell.SetupTempKnox(currentKnox);

        moves++;
        Moved?.Invoke(moves);
        UndoStateChanged?.Invoke(canUndo);
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
        RemoveKnoxsIfNeeded(currentKnox);
    }

    private void ClearCell(Cell cell)
    {
        if (cell == null) return;

        cell.ClearAllConnections();
        cell.IsOccupiedCell = KnoxColorType.None;
        if (cell.CellType == CellType.TempKnox)
            cell.ClearTempKnox();
    }

    private void ClearLevel()
    {
        if (isCompleted) return;

        isCompleted = false;
        canUndo = false;
        moves = 0;
        currentKnox = KnoxColorType.None;
        connectedKnoxs.Clear();
        undoSnapshots.Clear();
        foreach (var knox in knoxDict)
        {
            currentPath = knox.Value;
            ClearCurrentPath(currentPath);
        }

        KnoxsConnected?.Invoke(connectedKnoxs.Count);
        Moved?.Invoke(moves);
        UndoStateChanged?.Invoke(canUndo);
    }

    private bool CanUseUndo()
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
        moves--;
        Moved?.Invoke(moves);
        UndoStateChanged?.Invoke(canUndo);
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

        KnoxsConnected?.Invoke(connectedKnoxs.Count);
    }

    private void RemoveKnoxsIfNeeded(KnoxColorType currentKnox)
    {
        if (connectedKnoxs.Contains(currentKnox))
        {
            connectedKnoxs.Remove(currentKnox);
            KnoxsConnected?.Invoke(connectedKnoxs.Count);
        }
    }
}
