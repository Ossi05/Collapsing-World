using System;
using System.Collections;
using UnityEngine;

public class CollectableManager : MonoBehaviour {
    [SerializeField] GameObject collectablePrefab;
    [SerializeField] float spawnDelay = .5f;
    [SerializeField] int spawnAmt = 1;
    [SerializeField] int maxCollectAmt = 5;

    int collectedAmt = 0;

    public static CollectableManager Instance;

    public event EventHandler OnAllCollected;
    public event EventHandler OnCollected;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    public int GetMaxCollectAmt()
    {
        return maxCollectAmt;
    }

    public int GetCollectedAmt()
    {
        return collectedAmt;
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            SpawnCollectables();
        }

    }

    GridPosition? GetRandomSpawnGridPosition()
    {
        GridPosition? spawnGridPosition;
        int maxTries = 1000;
        int tries = 0;
        do
        {
            tries++;
            if (tries == maxTries)
            {
                Debug.LogWarning("MAX TRIES REACHED");
            }
            spawnGridPosition = LevelGrid.Instance.GetRandomSafeGridPosition();

            if (!IsSpawnAllowed())
            {
                return null;
            }
        }
        while (tries < maxTries &&
            (spawnGridPosition == Player.Instance.GetGridPosition() || (spawnGridPosition != null &&
            LevelGrid.Instance.IsGridPositionDestroyed(spawnGridPosition.Value))));
        return spawnGridPosition;
    }

    bool IsSpawnAllowed()
    {
        return LevelGrid.Instance.IsSafePositionsAvailable() && !GameManager.Instance.IsGameOver();
    }

    private void SpawnCollectableAtPosition(Collectable collectable, GridPosition position)
    {
        if (position == null) { return; }

        collectable.transform.position = LevelGrid.Instance.GetWorldPosition(position);
        LevelGrid.Instance.AddGridObjectAtGridPosition(position, collectable);
        collectable.Show();
    }

    public void SpawnCollectables()
    {
        if (!IsSpawnAllowed())
            return;

        for (int i = 0; i < spawnAmt; i++)
        {
            GridPosition? nullablePosition = GetRandomSpawnGridPosition();

            if (nullablePosition == null)
            {
                return;
            }

            GridPosition spawnGridPosition = nullablePosition.Value;
            GameObject collectableGameobject = Instantiate(collectablePrefab, Vector3.zero, Quaternion.identity);
            Collectable collectable = collectableGameobject.GetComponent<Collectable>();
            collectable.OnCollected += Collectable_OnCollected;
            collectable.OnPickUpFailed += Collectable_OnPickUpFailed;

            SpawnCollectableAtPosition(collectable, spawnGridPosition);
        }
    }

    IEnumerator DelaySpawn(Collectable collectable)
    {
        yield return new WaitForSeconds(spawnDelay);

        if (!IsSpawnAllowed())
            yield break;

        GridPosition? nullablePosition = GetRandomSpawnGridPosition();
        if (nullablePosition == null)
        {
            yield break;
        }
        GridPosition spawnGridPosition = nullablePosition.Value;
        SpawnCollectableAtPosition(collectable, spawnGridPosition);
    }

    void SpawnCollectable(Collectable collectable)
    {
        if (collectedAmt >= maxCollectAmt)
        {
            OnAllCollected?.Invoke(this, EventArgs.Empty);
            return;
        }
        StartCoroutine(DelaySpawn(collectable));
    }

    private void Collectable_OnCollected(object sender, EventArgs e)
    {
        HandleCollectableEvent(sender, wasCollected: true);
    }

    private void Collectable_OnPickUpFailed(object sender, EventArgs e)
    {
        HandleCollectableEvent(sender, wasCollected: false);
    }

    private void HandleCollectableEvent(object sender, bool wasCollected)
    {
        Collectable collectable = sender as Collectable;
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(collectable.transform.position);
        LevelGrid.Instance.RemoveGridObjectAtGridPosition(gridPosition, collectable);
        if (wasCollected)
        {
            collectedAmt++;
            OnCollected?.Invoke(this, EventArgs.Empty);
        }
        collectable.Hide();
        SpawnCollectable(collectable);
    }



}
