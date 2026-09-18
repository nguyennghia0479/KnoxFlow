using UnityEngine;
using UnityEngine.UI;

public class CellBuilder : MonoBehaviour
{
    [SerializeField] private Image knoxImg;
    [SerializeField] private Image tempKnoxImg;

    private int coordX;
    private int coordY;
    private CellType cellType;
    private KnoxColorType knoxColorType;
    private KnoxColorType isHintPath;

    public void SetupCell(int coordX, int coordY)
    {
        this.coordX = coordX;
        this.coordY = coordY;
        gameObject.name = $"Cell_{coordX}_{coordY}";
        cellType = CellType.None;
        isHintPath = KnoxColorType.None;
        UpdateVisual();
    }

    public void SetKnox(KnoxColorType knoxColorType)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(this, "Change Knox Type");
#endif

        this.knoxColorType = knoxColorType;
        knoxImg.color = HelperUtility.GetColorByType(knoxColorType);
        cellType = knoxColorType != KnoxColorType.None ? CellType.Knox : CellType.None;
        UpdateVisual();
    }

    public void SetHint(KnoxColorType knoxColorType)
    {
        this.knoxColorType = knoxColorType;
        tempKnoxImg.color = HelperUtility.GetColorByType(knoxColorType);
        isHintPath = knoxColorType;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (knoxImg == null)
        {
            Debug.LogError("[CellBuilder] have not assigned Knox Image");
            return;
        }

        bool isActive = cellType != CellType.None;
        knoxImg.gameObject.SetActive(isActive);
        tempKnoxImg.gameObject.SetActive(isHintPath != KnoxColorType.None);
    }

    public int CoordX => coordX;
    public int CoordY => coordY;
    public CellType CellType => cellType;
    public KnoxColorType KnoxColorType => knoxColorType;
}
