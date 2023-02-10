using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    public GameObject obstacle;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    public float timeBetweenSpawn;
    private float spawnTime;

    // There should be a delay before emit, to wait until loading animation finishes
    private bool isReadyToEmit = false;
    public float waitBeforeStart;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
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

        Instantiate(
            obstacle, 
            transform.position + new Vector3(randomX, randomY, 0), 
            Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))) // Random Z rotation https://forum.unity.com/threads/instantiate-with-a-random-y-rotation.345052/
        );
    }
}
