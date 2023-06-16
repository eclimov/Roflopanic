using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject obstaclePrefab;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    private  float timeBetweenSpawn;

    private float spawnTime;

    // There should be a delay before emit, to wait until loading animation finishes
    private bool isReadyToEmit = false;

    private LevelLoader levelLoader;
    private byte poolSize = 5; 
    private Queue<GameObject> pool;
    private GameObject coin;
    private WaitForSeconds cachedWaitForSecondsBeforeCoinSpawn;

    private Transform myTransform;

    private float coinSpawnChance;

    private void Awake()
    {
        // For Optimization purposes
        myTransform = transform;
    }

    // Start is called before the first frame update
    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.OnLevelLoad += SetEmitterReady;

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
    }

    protected void OnDestroy()
    {
        levelLoader.OnLevelLoad -= SetEmitterReady;
    }

    private void SetEmitterReady()
    {
        isReadyToEmit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isReadyToEmit && Time.time > spawnTime)
        {
            Spawn();
            spawnTime = Time.time + timeBetweenSpawn;
        }
    }

    void Spawn()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = myTransform.position + new Vector3(randomX, randomY, 0);

        pool.Enqueue(objectToSpawn);

        if(
            Random.value <= coinSpawnChance // Chance of spawning a coin
            && !coin.activeInHierarchy
        )
        {
            StartCoroutine(SpawnCoinDelayed());
        }
    }

    IEnumerator SpawnCoinDelayed()
    {
        yield return cachedWaitForSecondsBeforeCoinSpawn;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        coin.SetActive(true);
        coin.transform.position = myTransform.position + new Vector3(randomX, randomY, 0);
    }
}
