using System;
using System.Collections;
using UnityEngine;

public class BaseCube : MonoBehaviour {
    [SerializeField] protected int rollSpeed = 300;
    [SerializeField] protected int preGameMoveSpeed = 300;
    [SerializeField] protected int stepsToMove = 1;

    public event EventHandler OnStartRoll;

    protected bool isRolling;

    int currentSpeed;

    void Awake()
    {
        currentSpeed = preGameMoveSpeed;
    }

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            currentSpeed = rollSpeed;
        }
    }

    protected IEnumerator Roll(Direction rollDirection)
    {
        Vector3 direction = GetRollWorldDirection(rollDirection);
        isRolling = true;
        OnStartRoll?.Invoke(this, EventArgs.Empty);

        float cellSize = LevelGrid.Instance.GetCellSize();
        float remainingAngle = 90;
        Vector3 rotationCenter = transform.position + direction / 2 + Vector3.down / 2;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while (remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(currentSpeed * Time.deltaTime, remainingAngle);
            transform.RotateAround(rotationCenter, rotationAxis, rotationAngle);
            remainingAngle -= rotationAngle;
            yield return null;
        }

        isRolling = false;
    }

    protected Vector3 GetRollWorldDirection(Direction rollDirection)
    {
        Vector3 direction = new Vector3();
        switch (rollDirection)
        {
            case Direction.Up:
                direction = Vector3.forward * stepsToMove;
                break;
            case Direction.Down:
                direction = Vector3.back;
                break;
            case Direction.Left:
                direction = Vector3.left;
                break;
            case Direction.Right:
                direction = Vector3.right;
                break;
            default:
                Debug.LogError("Unknown Direction");
                break;
        }
        return direction;
    }
}
