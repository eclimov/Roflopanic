using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBuldigaBullet : MonoBehaviour
{
    public AudioClip bloodSplashSound;
    public GameObject bloodSplashCirclePrefab;
    public GameObject bloodSplashSidePrefab;
    public GameObject circleParticleEffectPrefab;

    private Rigidbody2D rb;

    private WaitForSeconds cachedWaitForSeconds;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cachedWaitForSeconds = new WaitForSeconds(.03f);

        StartCoroutine(DestroyDelayed(4f));
        StartCoroutine(SpawnCircleParticleRepeating());
    }

    private IEnumerator DestroyDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private IEnumerator SpawnCircleParticleRepeating()
    {
        while (true)
        {
            yield return cachedWaitForSeconds;
            GameObject circleParticleEffect = Instantiate(circleParticleEffectPrefab, GameObject.Find("Canvas").transform);
            circleParticleEffect.transform.position = transform.position;
        }
    }

    public void ThrowAt(Vector2 direction)
    {
        rb.AddForce(direction * 1000);
        rb.AddTorque(-300); // Rotation
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Boss>(out Boss boss))
        {
            boss.TakeDamage(Random.Range(7, 10));

            DestroyWithSideExplosion();
        }
        if (collision.TryGetComponent<MiniBuldiga>(out MiniBuldiga miniBuldiga))
        {
            miniBuldiga.TriggerDeath();

            DestroyWithSideExplosion();
        }
    }

    private void DestroyWithSideExplosion()
    {
        AudioManager.instance.PlaySound(bloodSplashSound);
        DestroyWithExplosion(bloodSplashSidePrefab);
    }

    public void DestroyWithCircleExplosion()
    {
        DestroyWithExplosion(bloodSplashCirclePrefab);
    }

    private void DestroyWithExplosion(GameObject explosionPrefab)
    {
        GameObject bloodSplashGameObject = Instantiate(explosionPrefab, GameObject.Find("Canvas UI").transform);
        bloodSplashGameObject.transform.position = gameObject.transform.position;

        Destroy(gameObject);
    }
}
