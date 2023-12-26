using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusProjectile : MonoBehaviour
{
    public GameObject explosionPrefab;
    public AudioClip hitSound;

    private Rigidbody2D rb;

    private string emittingPlayerName;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(DestroyDelayed(2f));
    }

    private IEnumerator DestroyDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void SetColor(Color32 color)
    {
        GetComponent<Image>().color = color;
    }

    public void ThrowAt(Vector2 direction)
    {
        rb.AddForce(direction * 2000);
    }

    public void SetEmittingPlayerName(string name)
    {
        emittingPlayerName = name;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<VersusPlayer>(out VersusPlayer versusPlayer))
        {
            if (emittingPlayerName != versusPlayer.gameObject.name)
            {
                AudioManager.instance.PlaySound(hitSound);
                versusPlayer.TakeDamage(Random.Range(5, 11));

                DestroyWithExplosion();
            }
        }
    }

    public void DestroyWithExplosion()
    {
        GameObject snowSplashGameObject = Instantiate(explosionPrefab, GameObject.Find("Canvas UI").transform);

        //Get the Particle System from the new GameObject
        ParticleSystem PartSystem = snowSplashGameObject.GetComponent<ParticleSystem>();
        //Get the MainModule of the ParticleSystem
        ParticleSystem.MainModule ma = PartSystem.main;
        ma.startColor = gameObject.GetComponent<Image>().color;

        snowSplashGameObject.transform.position = gameObject.transform.position;

        Destroy(gameObject);
    }
}
