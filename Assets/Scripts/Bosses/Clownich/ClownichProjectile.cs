using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClownichProjectile : MonoBehaviour
{
    public GameObject explosionPrefab;
    public AudioClip bossShieldDownSound;

    private Rigidbody2D rb;

    Vector2 lastVelocity;

    private bool isReady; // Is projectile exited spawn point area (Boss collision)?
    private bool isAllyGateCollisionTriggered; // A workaround, to prevent cases where multiple gates are collided at once, during a single tick

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float velocityX;
        float velocityY_abs;
        switch (SettingsManager.difficultyId)
        {
            case 0: // Easy
                velocityX = -13f;
                velocityY_abs = 13f;
                break;
            case 1: // Medium
                velocityX = -15f;
                velocityY_abs = 15f;
                break;
            default: // Hard
                velocityX = -20f;
                velocityY_abs = 15f;
                break;
        }
        rb.velocity = new Vector2(velocityX, Random.Range(-velocityY_abs, velocityY_abs));
    }

    public bool IsReady()
    {
        return isReady;
    }

    public void SetReady()
    {
        isReady = true;
    }

    public void SetColor(Color32 color)
    {
        GetComponent<Image>().color = color;
    }

    private void FixedUpdate()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var speed = lastVelocity.magnitude;
        var direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);

        if (collision.gameObject.tag == "Player")
        {
            speed = 20f;
            direction = new Vector2(1, direction.y);
        }

        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Border" || collision.gameObject.tag == "Projectile")
        {
            AudioManager.instance.PlayBallBounceSound();
            rb.velocity = direction * Mathf.Max(speed, 0f);
        }

        if (collision.gameObject.TryGetComponent(out ClownichGate clownichGate))
        {
            if(clownichGate.IsCleaning())
            {
                Destroy(gameObject);
            } else
            {
                ClownichBossGameManager clownichBossGameManager = FindObjectOfType<ClownichBossGameManager>();

                if(clownichGate.IsAlly() && !isAllyGateCollisionTriggered)
                {
                    clownichBossGameManager.DisableNextAllyGate();
                    isAllyGateCollisionTriggered = true;
                }

                if (clownichGate.IsEnemy())
                {
                    AudioManager.instance.PlaySound(bossShieldDownSound);
                    clownichBossGameManager.MakeBossVulnerable();
                }

                DestroyWithExplosion();
            }
        }
    }

    public void DestroyWithExplosion()
    {
        AudioManager.instance.AxeHitWoodAttemptSound();

        GameObject explosionGameObject = Instantiate(explosionPrefab, FindObjectOfType<ClownichBossGameManager>().gameObject.transform);
        explosionGameObject.transform.position = gameObject.transform.position;

        Destroy(gameObject);
    }
}
