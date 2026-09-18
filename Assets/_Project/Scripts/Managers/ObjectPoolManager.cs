using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Transform holder;
    [SerializeField] private int defaultSize = 50;
    [SerializeField] private int maxSize = 100;

    private IObjectPool<Cell> pool;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        pool = new ObjectPool<Cell>(
                    createFunc: CreateCell,
                    actionOnGet: OnTakeFromPool,
                    actionOnRelease: OnReleaseToPool,
                    actionOnDestroy: OnDestroyPool,
                    defaultCapacity: defaultSize,
                    maxSize: maxSize,
                    collectionCheck: true
                    );

        Cell[] tempArray = new Cell[defaultSize];
        for (int i = 0; i < tempArray.Length; i++)
            tempArray[i] = pool.Get();

        for (int i = 0; i < tempArray.Length; i++)
            pool.Release(tempArray[i]);
    }

    public Cell GetPool()
    {
        return pool.Get();
    }

    private Cell CreateCell()
    {
        Cell newCell = Instantiate(cellPrefab, holder);
        newCell.SetupPool(pool);
        return newCell;
    }

    private void OnTakeFromPool(Cell cell)
    {
        cell.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(Cell cell)
    {
        cell.gameObject.SetActive(false);
    }

    private void OnDestroyPool(Cell cell)
    {
        if (cell == null || cell.gameObject == null)
            return;

        Destroy(cell.gameObject);
    }
}
