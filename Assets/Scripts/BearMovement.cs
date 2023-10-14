using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 lastVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        AddRandomForceHorizontal();
        AddRandomForceVertical();
        AddRandomTorque();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var speed = lastVelocity.magnitude;
        var direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);
        rb.velocity = direction * Mathf.Max(speed, 0f);

        AddRandomTorque();
    }

    private void FixedUpdate()
    {
        lastVelocity = rb.velocity;

        rb.velocity *= .999f;
        rb.angularVelocity *= .999f;
    }

    private void AddRandomTorque()
    {
        float torque = Random.Range(0, 2) == 0 ? .02f : (-.02f);
        rb.AddTorque(torque);
    }

    private void AddRandomForceHorizontal()
    {
        Vector2 randomForceHorizontalDirection = Random.Range(0, 2) == 0 ? Vector2.left : Vector2.right;
        rb.AddForce(randomForceHorizontalDirection * .1f);
    }

    private void AddRandomForceVertical()
    {
        Vector2 randomForceHorizontalDirection = Random.Range(0, 2) == 0 ? Vector2.up : Vector2.down;
        rb.AddForce(randomForceHorizontalDirection * .1f);
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            AddRandomForceHorizontal();
            AddRandomForceVertical();
            AddRandomTorque();
        }
    }
}
