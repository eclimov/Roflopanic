using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    public float timeBetweenSpawn;

    private float spawnTime;

    // There should be a delay before emit, to wait until loading animation finishes
    private bool isReadyToEmit = false;
    public float waitBeforeStart;

    private byte poolSize = 5; 
    private Queue<GameObject> pool;

    private Transform myTransform;

    private void Awake()
    {
        // For Optimization purposes
        myTransform = transform;
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        pool = new Queue<GameObject>();
        for (byte i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        yield return new WaitForSeconds(waitBeforeStart);
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
    }
}
