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

    public void Die()
    {
        animator.SetTrigger("Die");
        isDead = true;

        AudioManager.instance.PlaySound(screamSound);
    }

    public void TakeDamage(int damage)
    {
        if (OnDamageTaken != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            // OnDamageTaken(50);
            OnDamageTaken(damage);
        }
    }

    public abstract void MakeVulnerable();
}
