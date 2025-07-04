using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject obstaclePrefab;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;

    private float timeBetweenSpawn;
    private float spawnTime;

    // There should be a delay before emit, to wait until loading animation finishes
    private bool isObstacleSpawnEnabled;
    private bool isCoinSpawnEnabled;

    private LevelLoader levelLoader;
    private byte poolSize = 5; 
    private Queue<GameObject> pool;
    private GameObject coin;
    private WaitForSeconds cachedWaitForSecondsBeforeCoinSpawn;

    private Transform myTransform;

    private float coinSpawnChance;
    private bool isCoinSpawnReady;

    private void Awake()
    {
        // For Optimization purposes
        myTransform = transform;
    }

    // Start is called before the first frame update
    private void Start()
    {
        levelLoader = FindAnyObjectByType<LevelLoader>();
        levelLoader.OnLevelLoad += StartSpawn;

        timeBetweenSpawn = SettingsManager.instance.GetDifficultyMap().obstacleTimeBetweenSpawn;

        pool = new Queue<GameObject>();
        for (byte i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        coin = Instantiate(coinPrefab);
        coin.SetActive(false);
        cachedWaitForSecondsBeforeCoinSpawn = new WaitForSeconds(timeBetweenSpawn / 2);

        coinSpawnChance = SettingsManager.GetCoinChance() / 100f;
        StartCoroutine(CoinSpawnCooldown(SettingsManager.instance.GetDifficultyMap().coinSpawnCooldownSeconds));
    }

    protected void OnDestroy()
    {
        levelLoader.OnLevelLoad -= StartSpawn;
    }

    public void StartSpawn()
    {
        isObstacleSpawnEnabled = true;
        isCoinSpawnEnabled = true;
    }

    public void StopSpawn()
    {
        isObstacleSpawnEnabled = false;
        isCoinSpawnEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > spawnTime)
        {
            if(isObstacleSpawnEnabled)
            {
                SpawnObstacle();
            }
            spawnTime = Time.time + timeBetweenSpawn;

            if(isCoinSpawnEnabled && isCoinSpawnReady)
            {
                StartCoroutine(SpawnCoinDelayed());
            }
        }
    }

    private void SpawnObstacle()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = myTransform.position + new Vector3(randomX, randomY, 0);

        pool.Enqueue(objectToSpawn);
    }

    private void ResetCoinSpawnTimeCooldown() {
        isCoinSpawnReady = false;
    }

    private IEnumerator CoinSpawnCooldown(int seconds) // To make sure a coin is not spawned too often
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            if (Random.value <= coinSpawnChance && !coin.activeSelf) // Keep this condition here
            {
                isCoinSpawnReady = true;
            }
        }
    }

    IEnumerator SpawnCoinDelayed()
    {
        yield return cachedWaitForSecondsBeforeCoinSpawn;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        if(isCoinSpawnEnabled) // This check fixes bug when a coin is spawned during reincarnation flow
        {
            coin.SetActive(true);
            coin.transform.position = myTransform.position + new Vector3(randomX, randomY, 0);

            ResetCoinSpawnTimeCooldown();
        }
    }
}
