using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBuldigaSpawnPoint : MonoBehaviour
{
    public GameObject miniBuldigaPrefab;

    private bool canSpawn;

    private Vector3 playerPosition;

    private float minSpawnRateDelay;
    private float maxSpawnRateDelay;

    private float minSpeed;
    private float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        switch (SettingsManager.difficultyId)
        {
            case 1: // Medium
                minSpawnRateDelay = .4f;
                maxSpawnRateDelay = .9f;

                minSpeed = 2.7f;
                maxSpeed = 3.3f;
                break;
            case 2: // Hardcore
                minSpawnRateDelay = .3f;
                maxSpawnRateDelay = .8f;

                minSpeed = 3f;
                maxSpeed = 3.5f;
                break;
            default: // Easy
                minSpawnRateDelay = .7f;
                maxSpawnRateDelay = 1.2f;

                minSpeed = 2.3f;
                maxSpeed = 2.8f;
                break;
        }

        StartCoroutine(SpawnMiniBuldigaRepeating());
    }

    public void PauseSpawn()
    {
        canSpawn = false;
    }

    public void ResumeSpawn()
    {
        canSpawn = true;
    }

    private IEnumerator SpawnMiniBuldigaRepeating()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnRateDelay, maxSpawnRateDelay));

            if(canSpawn)
            {
                int goodRateCoefficient;
                switch (SettingsManager.difficultyId)
                {
                    case 1: // Medium
                        goodRateCoefficient = 8;
                        break;
                    case 2: // Hardcore
                        goodRateCoefficient = 9;
                        break;
                    default: // Easy
                        goodRateCoefficient = 5;
                        break;
                }

                // goodRateCoefficient = 1; // NOTE: uncomment the folloiwng for debug purposes

                SpawnMiniBuldiga(
                    Random.Range(minSpeed, maxSpeed), // Speed
                    Random.Range(0, goodRateCoefficient) == 0 // Is good?
                );
            }
        }
    }

    public void SpawnMiniBuldiga(float speed, bool isGood)
    {
        playerPosition = FindAnyObjectByType<Player>().gameObject.transform.position;

        GameObject miniBuldigaGameObject = Instantiate(miniBuldigaPrefab, gameObject.transform);
        miniBuldigaGameObject.transform.position += Vector3.up * Random.Range(-10, 10);

        MiniBuldiga miniBuldiga = miniBuldigaGameObject.GetComponent<MiniBuldiga>();
        miniBuldiga.SetIsGood(isGood);
        miniBuldiga.SetSpeed(speed);
        miniBuldiga.SetTargetDirection(playerPosition);
    }
}
