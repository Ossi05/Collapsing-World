using System;
using UnityEngine;

public class Player : BaseGridItem {

    public static Player Instance { get; private set; }

    GridPosition currentGridPosition;
    bool isDead;
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnPlayerStartRoll;
    Rigidbody rb;
    PlayerMovement playerMovement;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        playerMovement.OnStartRoll += PlayerMovement_OnRolled;
    }

    void PlayerMovement_OnRolled(object sender, EventArgs e)
    {
        OnPlayerStartRoll?.Invoke(this, EventArgs.Empty);
    }

    public void SetStartingGridPosition(GridPosition gridPosition)
    {
        if (LevelGrid.Instance.IsValidGridPosition(gridPosition))
        {
            LevelGrid.Instance.AddGridObjectAtGridPosition(currentGridPosition, this);
        }
        currentGridPosition = gridPosition;
        transform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);

    }
    public void MoveToGridPosition(GridPosition gridPosition)
    {
        if (gridPosition == currentGridPosition) return;
        playerMovement.MoveToGridPosition(gridPosition);
    }

    public GridPosition GetGridPosition()
    {
        return currentGridPosition;
    }

    public void UpdateGridPosition(GridPosition newGridPosition)
    {
        bool isValidGridPosition = LevelGrid.Instance.IsValidGridPosition(newGridPosition);
        if (!isValidGridPosition || LevelGrid.Instance.IsGridPositionDestroyed(newGridPosition))
        {
            // Out of Range or Grid position is destroyed
            if (GameManager.Instance.IsGamePlaying())
            {
                HandleDeath();
            }
            return;
        }
        LevelGrid.Instance.GridObjectMovedGridPosition(this, currentGridPosition, newGridPosition);
        currentGridPosition = newGridPosition;
    }

    void HandleDeath()
    {
        if (isDead) { return; }
        isDead = true;
        rb.isKinematic = false;
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public override void HandleGridParentDeath()
    {
        HandleDeath();
    }

    public override void HandleGridParentIsSafeChange(bool isSafe) { }
}
