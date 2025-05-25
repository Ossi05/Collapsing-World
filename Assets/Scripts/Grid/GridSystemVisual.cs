using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour {
    public static GridSystemVisual Instance;

    [SerializeField] Transform gridSystemVisualSingle;
    [SerializeField] Transform outOfBoundsGridVisualSingle;

    [Header("Loading")]
    [SerializeField] float gridVisualSingleCreateDelay = 0.01f;
    [SerializeField] float gridRowCreateDelay = .4f;
    [SerializeField] float gridOutOfBoundsDestroyDelay = .2f;
    [SerializeField] bool skipCreateAnimation = false;

    GridSystemVisualSingle[,] gridSystemVisualSingleArray;
    List<GridSystemVisualSingle> outOfBoundsGridVisualSingleList = new List<GridSystemVisualSingle>();

    public event EventHandler OnGridVisualCreated;
    GridPosition outOfBoundsGridVisualEndPosition;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        LevelGrid.Instance.OnGridPreDestroy += LevelGrid_OnGridPreDestroy;
        LevelGrid.Instance.OnGridDestroy += LevelGrid_OnGridDestroy;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCreatingGridVisual())
        {
            StartCoroutine(CreateVisuals());
        }
        if (GameManager.Instance.IsGamePlaying())
        {
            StartCoroutine(DestroyOutOfBoundsVisuals());
        }
    }

    IEnumerator DestroyOutOfBoundsVisuals()
    {
        foreach (GridSystemVisualSingle visual in outOfBoundsGridVisualSingleList)
        {
            visual.ShowPreDestroyVisual();
            yield return new WaitForSeconds(gridOutOfBoundsDestroyDelay);
            visual.ShowDestroyVisual();
        }
    }

    IEnumerator GenerateVisualPathFromOutsideMap(GridPosition startPath, GridPosition toPath)
    {
        Direction direction = LevelGrid.Instance.GetDirectionTowardsGridPosition(startPath, toPath);
        GridPosition currentGridPosition = startPath;
        yield return CreateOutOfBoundsGridVisual(startPath);
        while (direction != Direction.None)
        {
            currentGridPosition = LevelGrid.Instance.GetGridPositionFromDirection(currentGridPosition, direction);
            if (LevelGrid.Instance.IsValidGridPosition(currentGridPosition))
            {
                break;
            }
            yield return CreateOutOfBoundsGridVisual(currentGridPosition);
        }
        outOfBoundsGridVisualEndPosition = currentGridPosition;
    }

    IEnumerator CreateVisuals()
    {
        yield return GenerateVisualPathFromOutsideMap(
             GameManager.Instance.GetPlayerInitialGridPosition(),
             GameManager.Instance.GetGameStartGridPosition());

        int width = LevelGrid.Instance.GetWidth();
        int height = LevelGrid.Instance.GetHeight();

        gridSystemVisualSingleArray = new GridSystemVisualSingle[width, height];

        List<Coroutine> rowCoroutines = new List<Coroutine>();
        int centerX = outOfBoundsGridVisualEndPosition.x;
        for (int z = 0; z < width; z++)
        {

            for (int offset = 0; centerX - offset >= 0 || centerX + 1 + offset < width; offset++)
            {
                int leftX = centerX - offset;
                if (leftX >= 0)
                {
                    GridPosition currentGridPosition = new GridPosition(leftX, z);
                    CreateGridVisualAtGridPosition(currentGridPosition);
                }

                int rightX = centerX + 1 + offset;
                if (rightX < width)
                {
                    GridPosition currentGridPosition = new GridPosition(rightX, z);
                    CreateGridVisualAtGridPosition(currentGridPosition);
                }
                if (!skipCreateAnimation) { yield return new WaitForSeconds(gridVisualSingleCreateDelay); }
            }

            if (!skipCreateAnimation) { yield return new WaitForSeconds(gridRowCreateDelay); }
        }

        OnGridVisualCreated?.Invoke(this, EventArgs.Empty);
    }

    IEnumerator CreateOutOfBoundsGridVisual(GridPosition gridPosition)
    {
        Transform visual = Instantiate(outOfBoundsGridVisualSingle, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
        GridSystemVisualSingle gridSystemVisualSingle = visual.GetComponent<GridSystemVisualSingle>();
        outOfBoundsGridVisualSingleList.Add(gridSystemVisualSingle);
        if (!skipCreateAnimation) { yield return new WaitForSeconds(gridVisualSingleCreateDelay); }

    }

    void CreateGridVisualAtGridPosition(GridPosition gridPosition)
    {
        Transform gridVisual = Instantiate(
            gridSystemVisualSingle,
            LevelGrid.Instance.GetWorldPosition(gridPosition),
            Quaternion.identity,
            this.transform);

        gridSystemVisualSingleArray[gridPosition.x, gridPosition.z] = gridVisual.GetComponent<GridSystemVisualSingle>();
        ShowGridPosition(gridPosition);
        gridVisual.gameObject.name = $"{gridPosition.x}, {gridPosition.z}";
    }


    private void LevelGrid_OnGridDestroy(object sender, GridPosition gridPosition)
    {
        gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].ShowDestroyVisual();
    }

    void LevelGrid_OnGridPreDestroy(object sender, GridPosition gridPosition)
    {
        gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].ShowPreDestroyVisual();
    }

    void ShowGridPosition(GridPosition gridPosition)
    {
        gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
    }



}
