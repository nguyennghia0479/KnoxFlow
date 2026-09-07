#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] private RectTransform mainFrameRect;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private int cols;
    [SerializeField] private int rows;
    [SerializeField] private GameObject cellBuilderPrefab;
    [SerializeField] private LevelSO levelSO;

    private float cellSize;
    private CellBuilder[,] gridData;

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

    public void GenerateGrid()
    {
        if (cellBuilderPrefab == null)
        {
            Debug.LogError("[GridBuilder] not have assigned GridCell Prefab");
            return;
        }
     
        ResetGrid();
        SetupGridLayoutGroup();

        gridData = new CellBuilder[cols, rows];
        int gridSize = cols * rows;
        for (int i = 0; i < gridSize; i++)
        {
            int coordX = i % cols;
            int coordY = i / cols;
            GameObject newCellBuilder = PrefabUtility.InstantiatePrefab(cellBuilderPrefab, transform) as GameObject;
            Undo.RegisterCreatedObjectUndo(newCellBuilder, "Generate Grid");
            if (newCellBuilder.TryGetComponent<CellBuilder>(out var cell))
            {
                cell.SetupCell(coordX, coordY);
                gridData[coordX, coordY] = cell;
            }
        }
    }

    public void ResetGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);

        gridData = null;
    }

    public void SaveToLevelSO()
    {
        if (levelSO == null)
        {
            Debug.LogError("[GridBuilder] not have assigned LevelSO");
            return;
        }

        CellBuilder[] cellBuilders = GetComponentsInChildren<CellBuilder>();
        if (cellBuilders.Length <= 0)
        {
            Debug.Log("Nothing to save");
            return;
        }

        List<KnoxInfo> knoxs = new();
        foreach (var cellBuilder in cellBuilders)
        {
            if (cellBuilder.CellType == CellType.Knox)
            {
                KnoxInfo knox = new()
                {
                    coordX = cellBuilder.CoordX,
                    coordY = cellBuilder.CoordY,
                    knoxColorType = cellBuilder.KnoxColorType
                };
                knoxs.Add(knox);
            }
        }

        levelSO.SaveLevelSO(cols, rows, knoxs.ToArray());
        EditorUtility.SetDirty(levelSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"Save {levelSO.name} successfully");
    }

    public void LoadFromeLevelSO()
    {
        if (levelSO == null)
        {
            Debug.LogError("[GridBuilder] not have assigned LevelSO");
            return;
        }

        cols = levelSO.Cols;
        rows = levelSO.Rows;
        GenerateGrid();

        KnoxInfo[] knoxs = levelSO.Knoxs;
        foreach (var knox in knoxs)
        {
            CellBuilder cell = gridData[knox.coordX, knox.coordY];
            if (cell != null)
                cell.SetKnox(knox.knoxColorType);
        }

        Debug.Log($"Load {levelSO.name} successfully");
    }
}
#endif