using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapHapychProjectile : MonoBehaviour
{
    public AudioClip bloodSplashSound;
    public GameObject bloodSplashPrefab;
    public GameObject bloodSplashSmallPrefab;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb.linearVelocity = new Vector2(10, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Obstacle>(out Obstacle obstacle))
        {
            obstacle.gameObject.SetActive(false);
            DisableWithExplosion(bloodSplashPrefab);
        } else if (collision.TryGetComponent<Boss>(out Boss boss))
        {
            boss.TakeDamage(Random.Range(1, 3));
            DisableWithExplosion(bloodSplashSmallPrefab);
        } else if(collision.TryGetComponent<MiniBuldiga>(out MiniBuldiga miniBuldiga))
        {
            miniBuldiga.TriggerDeath();
            DisableWithExplosion(bloodSplashSmallPrefab);
        }
    }

    private void DisableWithExplosion(GameObject bloodSplash)
    {
        AudioManager.instance.PlaySound(bloodSplashSound);
        GameObject bloodSplashGameObject = Instantiate(bloodSplash, GameObject.Find("Canvas UI").transform);
        bloodSplashGameObject.transform.position = gameObject.transform.position;


        gameObject.SetActive(false);
    }
}
