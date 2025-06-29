using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buldiga : Boss
{
    public CountdownTimer countdownTimer;
    public Image redEyesImage;

    private float directionY;
    private Rigidbody2D rb;
    private float speed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        countdownTimer.OnTick += OnTimerTick;
    }

    protected void OnDestroy()
    {
        countdownTimer.OnTick -= OnTimerTick;
    }

    private void OnEnable()
    {
        speed = 5f; // Do net make it too fast, to make sure the boss doesn't exceed border limit during the animation
        directionY = Random.Range(0, 2) == 0 ? -1f : 1f;

        transform.position = new Vector2(transform.position.x, 0);
        animator.SetTrigger("Enter");

        SetRedEyesColorAlpha(0);
    }

    private void OnTimerTick(int timeRemains)
    {
        switch (timeRemains)
        {
            case 4:
                SetRedEyesColorAlpha(50);
                break;
            case 3:
                SetRedEyesColorAlpha(128);
                break;
            case 2: 
            case 1:
            case 0:
                SetRedEyesColorAlpha(255);
                break;
            default:
                SetRedEyesColorAlpha(0);
                break;
        }
    }

    private void SetRedEyesColorAlpha(int alphaColor)
    {
        redEyesImage.color = new Color32(255, 255, 255, (byte)alphaColor);
    }

    private void SetRandomSpeed()
    {
        speed = Random.Range(3f, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When collides with border, change direction
        if (collision.gameObject.tag == "Border")
        {
            directionY *= -1;
            SetRandomSpeed();
        }
    }

    public override void MakeVulnerable()
    {
        isVulnerable = true;
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = new Vector2(0, -5f);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, directionY * speed);
        }
    }

    public override void PlayDamageAnimation()
    {
        animator.SetTrigger("Damage");
    }
}
