using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerSpeed;
    public GameObject CrownSprite;

    private Rigidbody2D rb;
    private Vector3 mousePos;
    private Camera mainCam;

    private bool isColliding = false;

    private float directionY;

    void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        if(FindObjectOfType<SettingsManager>().IsTargetTotalScoreAchieved())
        {
            CrownSprite.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ScoreManager.scoreUpdateType = ScoreManager.ScoreUpdateTypesEnum.SECOND;

        // float directionY = Input.GetAxisRaw("Vertical");
        directionY = 0f;
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

            if(!isColliding) // Do not allow sticking to the wall and gaining points
            {
                ScoreManager.scoreUpdateType = ScoreManager.ScoreUpdateTypesEnum.FRAME;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }

    private void FixedUpdate()
	{
        rb.velocity = new Vector2(0, directionY * playerSpeed);
    }
}
