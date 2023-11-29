using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    public AudioClip screamSound;

    public Animator animator;

    protected bool isDead;

    public delegate void OnDamageTakenDelegate(int damage);
    public event OnDamageTakenDelegate OnDamageTaken;

    protected bool isVulnerable;

    public void Die()
    {
        animator.SetTrigger("Die");
        isDead = true;

        AudioManager.instance.PlaySound(screamSound);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void TakeDamage(int damage)
    {
        // Prevent taking any more damage accidentally (when the boss gets hit by a hap-hapych projectile, for example) and triggering animation, sound and events, if the boss is dead already
        if (isDead) return;

        if (OnDamageTaken != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            // OnDamageTaken(50);
            OnDamageTaken(damage);
        }
    }

    public void TriggerExit()
    {
        animator.SetTrigger("Exit");
    }

    public abstract void PlayDamageAnimation();

    public void MakeInvulnerable()
    {
        isVulnerable = false;
    }

    public abstract void MakeVulnerable();
}
