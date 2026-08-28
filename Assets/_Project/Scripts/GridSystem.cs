using UnityEngine;
using UnityEngine.UI;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private RectTransform mainFrameRect;
    [SerializeField] private int cols;
    [SerializeField] private int rows;

    private GridLayoutGroup gridLayoutGroup;
    private float cellSize;
    private Cell[,] gridData;

    private void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        SetupGridLayoutGroup();
        GenerateGrid();
    }

    private void SetupGridLayoutGroup()
    {
        Canvas.ForceUpdateCanvases();
        float heightFrame = mainFrameRect.rect.height;
        float widthFrame = mainFrameRect.rect.width;

        float heightCell = heightFrame / rows;
        float widthCell = widthFrame / cols;

        cellSize = Mathf.Min(heightCell, widthCell);
        gridLayoutGroup.constraintCount = cols;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }

    private void GenerateGrid()
    {
        gridData = new Cell[cols, rows];
        int gridCount = cols * rows;
        for (int i = 0; i < gridCount; i++)
        {
            int coordX = i % cols;
            int coordY = i / cols;

            Cell newCell = Instantiate(cellPrefab, transform);
            newCell.SetPosition(coordX, coordY);
            gridData[coordX, coordY] = newCell;
        }
    }
}
