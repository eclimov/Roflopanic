using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody2D rb;
    private Vector2 playerDirection;
    private Vector3 mousePos;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // float directionY = Input.GetAxisRaw("Vertical");

        float directionY = 0f;
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
        }

        playerDirection = new Vector2(0, directionY).normalized;
    }

	private void FixedUpdate()
	{
        rb.velocity = new Vector2(0, playerDirection.y * playerSpeed);
	}
}
