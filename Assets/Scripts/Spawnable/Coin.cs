using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : AbstractSpawnable
{
    Rigidbody2D rb;

    bool hasTarget;
    Vector3 targetPosition;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        hasTarget = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if(hasTarget)
        {
            Vector2 targetDirection = (targetPosition - myTransform.position).normalized;
            rb.velocity = new Vector2(targetDirection.x, targetDirection.y) * (moveSpeed / 2);
        }
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }
}
