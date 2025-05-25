using System.Collections;
using UnityEngine;

public class PlayerMovement : BaseCube {

    private void Update()
    {
        if (!GameManager.Instance.IsGamePlaying() || Player.Instance.GetIsDead())
        {
            return;
        }

        Vector2 moveInput = PlayerControls.Instance.moveInput;
        if (isRolling) { return; }
        Direction moveDirection = GetDirection(moveInput);

        switch (moveDirection)
        {
            case Direction.Up:
                HandleRoll(Direction.Up);
                break;
            case Direction.Down:
                HandleRoll(Direction.Down);
                break;
            case Direction.Left:
                HandleRoll(Direction.Left);
                break;
            case Direction.Right:
                HandleRoll(Direction.Right);
                break;
            case Direction.None:
            default:
                // No movement
                break;
        }
    }


    private void HandleRoll(Direction rollDirection)
    {
        StartCoroutine(HandleRollCoroutine(rollDirection));
    }

    private IEnumerator HandleRollCoroutine(Direction rollDirection)
    {
        Vector3 direction = GetRollWorldDirection(rollDirection);
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position + direction);
        yield return StartCoroutine(Roll(rollDirection));
        Player.Instance.UpdateGridPosition(newGridPosition);
    }

    private Direction GetDirection(Vector2 input)
    {
        if (input == Vector2.up)
            return Direction.Up;
        if (input == Vector2.down)
            return Direction.Down;
        if (input == Vector2.left)
            return Direction.Left;
        if (input == Vector2.right)
            return Direction.Right;

        return Direction.None;
    }

    public void MoveToGridPosition(GridPosition targetPosition)
    {
        StartCoroutine(MoveToGridPositionCoroutine(targetPosition));
    }

    private IEnumerator MoveToGridPositionCoroutine(GridPosition targetPosition)
    {
        while (Player.Instance.GetGridPosition() != targetPosition)
        {
            if (isRolling)
            {
                yield return null;  // wait until rolling is finished
                continue;
            }

            GridPosition currentPos = Player.Instance.GetGridPosition();
            Direction moveDirection = LevelGrid.Instance.GetDirectionTowardsGridPosition(currentPos, targetPosition);

            if (moveDirection == Direction.None)
            {
                yield break;  // no valid move needed
            }

            HandleRoll(moveDirection);

            // Wait until rolling completes
            while (isRolling)
            {
                yield return null;
            }
        }
    }

}
