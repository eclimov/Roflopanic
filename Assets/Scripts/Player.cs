using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject CrownSprite;

    private Rigidbody2D rb;
    private Vector3 mousePos;
    private Camera mainCam;

    private bool isCollidingBorder = false;

    private float directionY;

    private ScoreManager scoreManager;

    private float playerSpeed;

    void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        if(SettingsManager.instance.IsTargetTotalScoreAchieved())
        {
            CrownSprite.SetActive(true);
        }

        scoreManager = FindObjectOfType<ScoreManager>();

        playerSpeed = SettingsManager.instance.GetDifficultyMap().playerSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        directionY = 0f;

#if UNITY_EDITOR
        directionY = Input.GetAxisRaw("Vertical");
#endif

        if (Input.GetMouseButton(0)) // Same as touching the screen https://www.youtube.com/watch?v=0M-9EtUArhw
        {
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (
                mousePos.x > 1
                && mousePos.y < 3.4f // Non-interactive area for Pause button (the dimensions represent FOV: https://forum.unity.com/threads/screentoworldpoint-always-the-same.337105/)
            )
            {
                directionY = 1f;
            }
            else if (mousePos.x < -1)
            {
                directionY = -1f;
            }

            if(!isCollidingBorder) // Do not allow sticking to the wall and gaining points
            {
                scoreManager.IncreaseTargetScore();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            collision.gameObject.SetActive(false);
            scoreManager.OnCoinCollected();
            AudioManager.instance.PlayCoinSound();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            isCollidingBorder = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            isCollidingBorder = false;
        }
    }

    private void FixedUpdate()
	{
        rb.velocity = new Vector2(0, directionY * playerSpeed);
    }
}
