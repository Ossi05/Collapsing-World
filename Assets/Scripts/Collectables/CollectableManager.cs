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

    void SpawnCollectables()
    {
        for (int i = 0; i < spawnAmt; i++)
        {
            if (!LevelGrid.Instance.IsSafePositionsAvailable()) { return; }

            GridPosition spawnGridPosition = GetRandomSpawnGridPosition();
            while (spawnGridPosition == Player.Instance.GetGridPosition())
            {
                if (!LevelGrid.Instance.IsSafePositionsAvailable() || GameManager.Instance.IsGameOver()) { return; }

                spawnGridPosition = GetRandomSpawnGridPosition();
            }
            GameObject collectableGameobject = Instantiate(collectablePrefab, LevelGrid.Instance.GetWorldPosition(spawnGridPosition), Quaternion.identity);
            Collectable collectable = collectableGameobject.GetComponent<Collectable>();
            collectable.OnCollected += Collectable_OnCollected;
            collectable.OnPickUpFailed += Collectable_OnPickUpFailed;
            LevelGrid.Instance.AddGridObjectAtGridPosition(spawnGridPosition, collectable);
            collectable.Show();
        }
    }

    GridPosition GetRandomSpawnGridPosition()
    {
        return LevelGrid.Instance.GetRandomSafeGridPosition();
    }

    IEnumerator DelaySpawn(Collectable collectable)
    {
        yield return new WaitForSeconds(spawnDelay);
        GridPosition spawnGridPosition = GetRandomSpawnGridPosition();
        collectable.transform.position = LevelGrid.Instance.GetWorldPosition(spawnGridPosition);
        LevelGrid.Instance.AddGridObjectAtGridPosition(spawnGridPosition, collectable);
        collectable.Show();
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
