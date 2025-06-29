using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HapHapych : MonoBehaviour
{
    public AudioClip shootSound;
    public GameObject projectileGameObjectPrefab;

    private GameObject playerGameObject;
    private GameObject projectileGameObject;
    private WaitForSeconds cachedWaitForSecondsShootInterval;

    private void Awake()
    {
        // Do not initialize this in Start, because Start goes after Enable
        cachedWaitForSecondsShootInterval = new WaitForSeconds(8f); // Use scaled time here, to avoid incrementing score during pause

        playerGameObject = FindAnyObjectByType<Player>().gameObject;
    }

    private void OnEnable() // Works after reincarnation as well
    {
        if(SceneManager.GetActiveScene().name != "Shop")
        {
            StartCoroutine(InfiniteShooting());
        }
    }

    private IEnumerator InfiniteShooting()
    {
        while (true)
        {
            yield return cachedWaitForSecondsShootInterval;
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        AudioManager.instance.PlaySound(shootSound);
        if(projectileGameObject == null)
        {
            projectileGameObject = Instantiate(projectileGameObjectPrefab);
        } else
        {
            // Reactivate, to reset its velocity
            projectileGameObject.SetActive(false);
            projectileGameObject.SetActive(true);
        }
        projectileGameObject.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float rotationSpeed = 60;

        transform.RotateAround(playerGameObject.transform.position, Vector3.forward, (-1) * rotationSpeed * Time.deltaTime);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
