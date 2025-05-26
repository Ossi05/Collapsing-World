using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LevelGrid : MonoBehaviour {
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] float cellSize = 1;
    [SerializeField] int maxSafePathLenght = 75;
    [Header("Destroying Cells")]
    [SerializeField] float minSafePathToDestroyAll = 5;
    [SerializeField] float startDestroyingDelay = 1f;
    [SerializeField] float minIntervalBetweenDestructions = 0.2f;
    [SerializeField] float maxIntervalBetweenDestructions = 1f;
    [SerializeField] float destructionDelay = 3f;
    [Header("Debug")]
    [SerializeField] Transform gridDebugObjectPrefab;
    bool allowDebug = false; // Set true if allowed in build version

    public static LevelGrid Instance;
    public event EventHandler<GridPosition> OnGridPreDestroy;
    public event EventHandler<GridPosition> OnGridDestroy;

    GridSystem gridSystem;

    List<GridPosition> safeGridPositionsList = new List<GridPosition>();

    Coroutine destroyGridCoroutine;

    bool destroyAllPositions = false;

    void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        allowDebug = true;
#endif
        CreateGrid();
    }

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (allowDebug && Input.GetKeyDown(KeyCode.T))
        {
            gridSystem.ToggleShowDebugGridObjects();
        }
    }
#endif

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            List<GridPosition> destroyableGridPositions = GetDestroyableGridPositions();
            if (destroyableGridPositions.Count == 0) return;
            destroyGridCoroutine = StartCoroutine(DestroyGridObjectsRandomly(destroyableGridPositions));
        }
        if (GameManager.Instance.IsGameOver())
        {
            StopCoroutine(destroyGridCoroutine);
        }
    }

    void CreateGrid()
    {
        if (maxSafePathLenght > width * height)
        {
            Debug.LogError("maxSafePathLenght can't be bigger than grid size");
            return;
        }

        gridSystem = new GridSystem(width, height, cellSize);
        GridPosition startPosition = new GridPosition(
            UnityEngine.Random.Range(0, gridSystem.GetWidth()),
            UnityEngine.Random.Range(0, gridSystem.GetHeight())
        );
        Debug.Log(maxSafePathLenght);
        GenerateSafePath(startPosition, maxSafePathLenght);
        if (!allowDebug) { return; }
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    void GenerateSafePath(GridPosition startPosition, int maxLength)
    {
        List<GridPosition> newSafeGridPositionsList = new List<GridPosition>();

        if (!IsValidGridPosition(startPosition) || gridSystem.GetGridObject(startPosition).IsDestroyed())
        {
            Debug.Log("GenerateSafePath aborted: Invalid or destroyed start position.");
            ReplaceSafeGridPositionsList(newSafeGridPositionsList);
            return;
        }

        gridSystem.GetGridObject(startPosition).SetIsSafe(true);
        newSafeGridPositionsList.Add(startPosition);

        List<GridPosition> frontier = new List<GridPosition> { startPosition };
        GridPosition[] directions = new GridPosition[]
        {
        new(0, 1), new(0, -1),
        new(1, 0), new(-1, 0)
        };

        Dictionary<GridPosition, GridPosition?> parentMap = new();
        parentMap[startPosition] = null;

        int steps = 0;
        var rnd = new System.Random();

        int availablePositions = gridSystem.GetNotDestroyedPositions().Count;
        int effectiveMaxLength = Mathf.Min(maxLength, availablePositions);

        int maxLoopIterations = 10000;
        int loopCounter = 0;

        bool constraintsRelaxed = false;

        while (steps < effectiveMaxLength && frontier.Count > 0 && loopCounter < maxLoopIterations)
        {
            loopCounter++;

            if (loopCounter >= maxLoopIterations)
            {
                Debug.LogError("GenerateSafePath terminated due to max loop iterations. Potential logic issue.");
                break;
            }

            frontier = frontier.OrderBy(_ => rnd.Next()).ToList();
            List<GridPosition> newFrontier = new List<GridPosition>();
            int activeFrontierCount = Mathf.CeilToInt(frontier.Count * 0.7f);
            activeFrontierCount = Mathf.Clamp(activeFrontierCount, 0, frontier.Count);

            int positionsAddedThisLoop = 0;

            for (int i = 0; i < activeFrontierCount && steps < effectiveMaxLength; i++)
            {
                GridPosition origin = frontier[i];
                GridPosition? parent = parentMap.ContainsKey(origin) ? parentMap[origin] : null;

                int baseBranches = (steps < effectiveMaxLength * 0.3f) ? 3 : 2;
                int branches = rnd.Next(2, baseBranches + 2);
                GridPosition[] dirs = directions.OrderBy(_ => rnd.Next()).ToArray();
                int b = 0;

                foreach (GridPosition dir in dirs)
                {
                    if (b >= branches || steps >= effectiveMaxLength)
                        break;

                    GridPosition next = new GridPosition(origin.x + dir.x, origin.z + dir.z);

                    if (!IsValidGridPosition(next) || newSafeGridPositionsList.Contains(next))
                        continue;

                    GridObject gridObject = gridSystem.GetGridObject(next);
                    if (gridObject.IsDestroyed() || gridObject.IsBeingDestroyed())
                        continue;

                    if (!constraintsRelaxed && parent.HasValue)
                    {
                        GridPosition prevDir = new GridPosition(origin.x - parent.Value.x, origin.z - parent.Value.z);
                        if (prevDir.x == dir.x && prevDir.z == dir.z && rnd.NextDouble() < 0.6)
                            continue;
                    }

                    if (!constraintsRelaxed)
                    {
                        double skipChance = steps < effectiveMaxLength * 0.2f ? 0.05 : Mathf.Lerp(0.05f, 0.35f, steps / (float)effectiveMaxLength);
                        if (rnd.NextDouble() < skipChance)
                            continue;

                        int adjacentSafeCount = 0;
                        foreach (GridPosition d2 in directions)
                        {
                            GridPosition neighbor = new GridPosition(next.x + d2.x, next.z + d2.z);
                            if (newSafeGridPositionsList.Contains(neighbor))
                                adjacentSafeCount++;
                        }
                        if (steps < effectiveMaxLength * 0.3f && adjacentSafeCount > 2)
                            continue;
                        else if (adjacentSafeCount > 1)
                            continue;
                    }

                    newSafeGridPositionsList.Add(next);
                    newFrontier.Add(next);
                    gridObject.SetIsSafe(true);
                    parentMap[next] = origin;
                    steps++;
                    b++;
                    positionsAddedThisLoop++;
                }
            }

            if (positionsAddedThisLoop == 0)
            {
                if (!constraintsRelaxed)
                {
                    Debug.Log("Constraints too strict — relaxing constraints to prevent infinite loop.");
                    constraintsRelaxed = true;
                }
                else
                {
                    Debug.Log("Path generation halted: no more positions could be added even with relaxed constraints.");
                    break;
                }
            }

            frontier = newFrontier;
        }

        ReplaceSafeGridPositionsList(newSafeGridPositionsList);
    }


    void ReplaceSafeGridPositionsList(List<GridPosition> newSafeGridPositionsList)
    {
        if (safeGridPositionsList.Count > 0)
        {
            List<GridPosition> oldSafePositions = new List<GridPosition>(safeGridPositionsList);

            foreach (GridPosition gridPosition in oldSafePositions)
            {
                GridObject gridObject = gridSystem.GetGridObject(gridPosition);
                if (!newSafeGridPositionsList.Contains(gridPosition))
                {
                    gridObject.SetIsSafe(false);
                }
            }
        }

        safeGridPositionsList = newSafeGridPositionsList;
    }

    List<GridPosition> GetDestroyableGridPositions()
    {
        // Collect all destroyable positions
        List<GridPosition> destroyablePositions = new List<GridPosition>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition pos = new GridPosition(x, z);
                GridObject gridObject = gridSystem.GetGridObject(pos);
                if (gridObject.IsDestroyed() || gridObject.IsBeingDestroyed()) { continue; }
                if (!destroyAllPositions && gridObject.IsSafe())
                {
                    continue;
                }
                destroyablePositions.Add(pos);
            }
        }
        return destroyablePositions;
    }

    private IEnumerator DestroyGridObjectsRandomly(List<GridPosition> destroyablePositions)
    {
        if (destroyablePositions == null || destroyablePositions.Count == 0)
        {
            Debug.Log("No destroyable positions left. Stopping destruction.");
            yield break;
        }

        yield return new WaitForSeconds(startDestroyingDelay);

        destroyablePositions = destroyablePositions.OrderBy(p => UnityEngine.Random.value).ToList();

        for (int i = 0; i < destroyablePositions.Count; i++)
        {
            HandleGridDestroy(destroyablePositions[i]);

            float delay = UnityEngine.Random.Range(minIntervalBetweenDestructions, maxIntervalBetweenDestructions);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(destructionDelay);
        if (destroyAllPositions) { yield break; }

        // ALL DESTROYED. FIND NEW GRIDPOSITIONS TO DESTROY
        int newSafePathMaxLength = Mathf.RoundToInt(safeGridPositionsList.Count / 2);
        int availablePositions = gridSystem.GetNotDestroyedPositions().Count;

        if (newSafePathMaxLength >= availablePositions && newSafePathMaxLength > minSafePathToDestroyAll)
        {
            GenerateSafePath(Player.Instance.GetGridPosition(), newSafePathMaxLength);
        }
        else
        {
            destroyAllPositions = true;
            GenerateSafePath(Player.Instance.GetGridPosition(), availablePositions);
        }

        List<GridPosition> newDestroyableGridPositions = GetDestroyableGridPositions();
        yield return DestroyGridObjectsRandomly(newDestroyableGridPositions);
    }


    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();

    public float GetCellSize() => gridSystem.GetCellSize();

    public void AddGridObjectAtGridPosition(GridPosition gridPosition, BaseGridItem gridItem)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddGridItem(gridItem);
    }

    public List<BaseGridItem> GetGridObjectListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetGridItemsList();
    }

    public void RemoveGridObjectAtGridPosition(GridPosition gridPosition, BaseGridItem gridItem)
    {
        GridObject gridObjectList = gridSystem.GetGridObject(gridPosition);
        gridObjectList.RemoveGridItem(gridItem);
    }

    public void GridObjectMovedGridPosition(BaseGridItem gridItem, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        if (IsValidGridPosition(fromGridPosition))
        {
            RemoveGridObjectAtGridPosition(fromGridPosition, gridItem);
        }
        AddGridObjectAtGridPosition(toGridPosition, gridItem);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public bool IsGridPositionDestroyed(GridPosition gridPosition) => gridSystem.IsGridPositionDestroyed(gridPosition);
    public GridPosition GetCenterGridPosition() => gridSystem.GetCenterGridPosition();

    public bool IsSafePositionsAvailable()
    {
        return safeGridPositionsList.Count > 0;
    }

    public GridPosition? GetRandomSafeGridPosition()
    {
        if (!IsSafePositionsAvailable()) { return null; }
        return safeGridPositionsList[UnityEngine.Random.Range(0, safeGridPositionsList.Count)];
    }
    void HandleGridDestroy(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetIsBeingDestroyed();
        if (gridObject.IsSafe())
        {
            safeGridPositionsList.Remove(gridPosition);
        }
        gridObject.SetIsSafe(false);
        OnGridPreDestroy?.Invoke(this, gridPosition);
        StartCoroutine(DestroyGridPosition(gridPosition));
    }

    IEnumerator DestroyGridPosition(GridPosition gridPosition)
    {
        yield return new WaitForSeconds(destructionDelay);
        gridSystem.DestroyGridObject(gridPosition);
        OnGridDestroy?.Invoke(this, gridPosition);

    }

    public Direction GetDirectionTowardsGridPosition(GridPosition fromPosition, GridPosition toPosition) => gridSystem.GetDirectionTowardsGridPosition(fromPosition, toPosition);
    public GridPosition GetGridPositionFromDirection(GridPosition gridPosition, Direction direction) => gridSystem.GetGridPositionFromDirection(gridPosition, direction);
}
