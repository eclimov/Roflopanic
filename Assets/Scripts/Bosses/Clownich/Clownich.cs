using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clownich : Boss
{
    public ClownichProjectileSpawner clownichProjectileSpawner;
    public AudioClip laughSound;

    private float directionY;

    private Rigidbody2D rb;
    private float speed;

    private WaitForSecondsRealtime cachedWaitForSecondsRealtimeBetweenAccelerations;
    private WaitForSecondsRealtime cachedWaitForSecondsRealtimeBetweenAttacks;
    private WaitForSecondsRealtime cachedWaitForSecondsRealtimeBetweenLaughs;

    private bool isVulnerable;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        cachedWaitForSecondsRealtimeBetweenAccelerations = new WaitForSecondsRealtime(.5f);
        cachedWaitForSecondsRealtimeBetweenAttacks = new WaitForSecondsRealtime(1f);
        cachedWaitForSecondsRealtimeBetweenLaughs = new WaitForSecondsRealtime(8f);

        speed = 2f;
        directionY = Random.Range(0, 2) == 0 ? -1f : 1f;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2f);
        StartCoroutine(InfiniteSpeedRandomize());
        StartCoroutine(InfiniteAttackRandomize());
        StartCoroutine(InfiniteLaughRandomize());
    }

    private IEnumerator InfiniteSpeedRandomize()
    {
        for (; ; )
        {

            if (speed > 2f || animator.GetCurrentAnimatorStateInfo(0).IsName("Clownich_Laugh"))
            {
                speed = 2f;
            }
            else if (Random.Range(0, 4) == 0)
            {
                speed = 8f;
            }

            yield return cachedWaitForSecondsRealtimeBetweenAccelerations;
        }
    }

    private IEnumerator InfiniteAttackRandomize()
    {
        for (; ; )
        {
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Clownich_Laugh"))
            {
                if (Random.Range(0, 2) == 0)
                {
                    animator.SetTrigger("StartAttack");
                }
            }

            yield return cachedWaitForSecondsRealtimeBetweenAttacks;
        }
    }

    private IEnumerator InfiniteLaughRandomize()
    {
        for (; ; )
        {
            if (Random.Range(0, 2) == 0)
            {
                animator.SetTrigger("Laugh");
            }

            yield return cachedWaitForSecondsRealtimeBetweenLaughs;
        }
    }

    public void PlayLaughSound()
    {
        AudioManager.instance.PlaySound(laughSound);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When collides with border, change direction
        if(collision.gameObject.tag == "Border")
        {
            directionY *= -1;
        }

        if(collision.gameObject.TryGetComponent(out ClownichProjectile clownichProjectile))
        {
            if(clownichProjectile.IsReady())
            {
                if(isVulnerable)
                {
                    AudioManager.instance.AxeHitWoodSuccessfulSound();

                    animator.SetTrigger("Damage");
                    TakeDamage(Random.Range(5, 11));
                }

                clownichProjectile.DestroyWithExplosion();
            } else
            {
                clownichProjectile.SetReady();
            }
        }
    }

    public override void MakeVulnerable()
    {
        animator.SetTrigger("Damage");
        isVulnerable = true;

        AudioManager.instance.AxeHitWoodSuccessfulSound();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = new Vector2(0, -5f);
        } else
        {
            rb.velocity = new Vector2(0, directionY * speed);
        }
    }
}
