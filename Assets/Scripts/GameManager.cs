using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] float gameOverDelay = .5f;
    State state;
    GridPosition startGamePosition;
    GridPosition playerInitialGridPosition;
    int playerInitialZPosition = -5;

    bool gameWon = false;

    bool isGamePaused = false;

    enum State {
        WaitingToStart,
        CreatingGridVisuals,
        LoadingPlayer,
        GamePlaying,
        GameOver,
    }

    void Awake()
    {
        state = State.WaitingToStart;
        Instance = this;
        Instantiate(playerPrefab); // Spawn player
        Time.timeScale = 1f;
    }

    private void Start()
    {
        GridSystemVisual.Instance.OnGridVisualCreated += GridSystemVisual_OnGridVisualCreated;
        startGamePosition = LevelGrid.Instance.GetCenterGridPosition();
        playerInitialGridPosition = new GridPosition(startGamePosition.x, playerInitialZPosition);
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
        Player.Instance.SetStartingGridPosition(playerInitialGridPosition);
        PlayerControls.Instance.OnPauseAction += PlayerControls_OnPauseAction;
        //   CollectableManager.Instance.OnAllCollected += CollectableManager_OnAllCollected;
    }

    private void PlayerControls_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    void GridSystemVisual_OnGridVisualCreated(object sender, EventArgs e)
    {
        Player.Instance.MoveToGridPosition(startGamePosition);
        ChangeState(State.LoadingPlayer);
    }

    private void CollectableManager_OnAllCollected(object sender, EventArgs e)
    {
        gameWon = true;
        StartCoroutine(DelayGameOver());
    }

    void Player_OnPlayerDeath(object sender, EventArgs e)
    {
        StartCoroutine(DelayGameOver());
    }

    IEnumerator DelayGameOver() // Should be instant???
    {
        yield return new WaitForSeconds(gameOverDelay);
        ChangeState(State.GameOver);
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                ChangeState(State.CreatingGridVisuals);
                break;
            case State.CreatingGridVisuals:
                break;
            case State.LoadingPlayer:
                if (Player.Instance.GetGridPosition() == startGamePosition)
                {
                    ChangeState(State.GamePlaying);
                }
                break;
            case State.GamePlaying:
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }

    void ChangeState(State newState)
    {
        state = newState;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsWaitingToStart()
    {
        return state == State.WaitingToStart;
    }

    public bool IsCreatingGridVisual()
    {
        return state == State.CreatingGridVisuals;
    }

    public bool IsLoadingPlayer()
    {
        return state == State.LoadingPlayer;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public bool IsGameWon()
    {
        return gameWon;
    }

    public GridPosition GetGameStartGridPosition()
    {
        return startGamePosition;
    }

    public GridPosition GetPlayerInitialGridPosition()
    {
        return playerInitialGridPosition;
    }

    public void TogglePauseGame()
    {
        if (IsGameOver()) { return; }
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }

}