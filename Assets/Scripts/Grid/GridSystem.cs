using UnityEngine;

public class GridSystem {
    int width;
    int height;
    float cellSize;
    GridObject[,] gridObjectArray;
    GameObject gridDebugObjectsParent;

    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new GridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = new GridObject(gridPosition);
            }
        }

    }

    public GridPosition GetCenterGridPosition()
    {
        int centerX = (width - 1) / 2;
        int centerZ = (height - 1) / 2;
        return new GridPosition(centerX, centerZ);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize));
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        gridDebugObjectsParent = new GameObject("GridDebugObjects");
        gridDebugObjectsParent.transform.position = Vector3.zero;
        Transform parent = gridDebugObjectsParent.transform;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(
                    debugPrefab,
                    GetWorldPosition(gridPosition),
                    Quaternion.identity,
                    gridDebugObjectsParent.transform
                    );
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        bool gg = gridPosition.x >= 0 &&
            gridPosition.z >= 0 &&
            gridPosition.x < width &&
            gridPosition.z < height;
        return gridPosition.x >= 0 &&
            gridPosition.z >= 0 &&
            gridPosition.x < width &&
            gridPosition.z < height;
    }

    public void DestroyGridObject(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridObject(gridPosition);
        gridObject.DestroyGridItems();
        gridObject.SetIsDestroyed(true);
    }

    public bool IsGridPositionDestroyed(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridObject(gridPosition);
        return gridObject.GetIsDestroyed();
    }
    public Direction GetDirectionTowardsGridPosition(GridPosition fromPosition, GridPosition toPosition)
    {
        int deltaX = toPosition.x - fromPosition.x;
        int deltaZ = toPosition.z - fromPosition.z;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaZ))
        {
            return deltaX > 0 ? Direction.Right : Direction.Left;
        }
        else if (Mathf.Abs(deltaZ) > 0)
        {
            return deltaZ > 0 ? Direction.Up : Direction.Down;
        }

        return Direction.None;
    }

    public GridPosition GetGridPositionFromDirection(GridPosition gridPosition, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new GridPosition(gridPosition.x, gridPosition.z + 1);
            case Direction.Down:
                return new GridPosition(gridPosition.x, gridPosition.z - 1);
            case Direction.Left:
                return new GridPosition(gridPosition.x - 1, gridPosition.z);
            case Direction.Right:
                return new GridPosition(gridPosition.x + 1, gridPosition.z);
            default:
                Debug.LogError("Unknown Direction");
                return gridPosition;
        }
    }

    public void ToggleShowDebugGridObjects()
    {
        if (!gridDebugObjectsParent)
        {
            Debug.LogError("gridDebugObjectsParent hasn't been created");
        }
        gridDebugObjectsParent.SetActive(!gridDebugObjectsParent.activeSelf);
    }

}
