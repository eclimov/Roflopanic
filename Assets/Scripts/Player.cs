using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public RectTransform controlDown;
    public RectTransform controlUp;

    public GameObject crownSpriteGameObject;
    public GameObject magnetColliderGameObject;
    public GameObject hapHapychGameObject;

    private Rigidbody2D rb;
    private Vector3 mousePos;
    private Camera mainCam;

    private bool isCollidingBorder = false;

    private float directionY;

    private ScoreManager scoreManager;

    private float playerSpeed;
    private float rotationSpeed;

    void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        scoreManager = FindObjectOfType<ScoreManager>();

        playerSpeed = SettingsManager.instance.GetDifficultyMap().playerSpeed;
        rotationSpeed = playerSpeed * 25;

        SettingsManager.instance.OnEquippedItemsChange += LoadPlayerUI;
        LoadPlayerUI();
    }

    protected void OnDestroy()
    {
        SettingsManager.instance.OnEquippedItemsChange -= LoadPlayerUI;
    }

    private void LoadPlayerUI()
    {
        crownSpriteGameObject.SetActive(SettingsManager.IsAbilityEquipped("reincarnation"));
        magnetColliderGameObject.SetActive(SettingsManager.IsAbilityEquipped("magnet"));
        hapHapychGameObject.SetActive(SettingsManager.IsAbilityEquipped("haphapych"));
    }

    public void OnReincarnate()
    {
        crownSpriteGameObject.SetActive(false);
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
            // the dimensions represent FOV: https://forum.unity.com/threads/screentoworldpoint-always-the-same.337105/
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (RectTransformUtility.RectangleContainsScreenPoint(controlUp, mousePos))
            {
                directionY = 1f;
            } else if(RectTransformUtility.RectangleContainsScreenPoint(controlDown, mousePos))
            {
                directionY = -1f;
            }
        }

        // Score increase
        if (directionY != 0 && !isCollidingBorder && scoreManager != null) // Do not allow sticking to the wall and gaining points
        {
            scoreManager.IncreaseTargetScore();
        }

        // Rotation
        if (directionY == 0 || isCollidingBorder) 
        {
            rotatePlayer(0f);
        } else
        {
            rotatePlayer(directionY * 15f);
        }
    }

    private void rotatePlayer(float angle)
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.AngleAxis(angle, Vector3.forward),
            Time.deltaTime * rotationSpeed
        );
    }

    public void OnCoinCollected()
    {
        scoreManager.OnCoinCollected();
    }

    // It's called on each frame
    private void OnCollisionStay2D(Collision2D collision)
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
