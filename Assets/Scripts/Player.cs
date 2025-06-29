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

    private bool canMove = true;
    private bool isVulnerable = true;

    void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        scoreManager = FindAnyObjectByType<ScoreManager>();

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

    public void ToggleMovement(bool status)
    {
        rb.linearVelocity *= 0; // Reset velocity
        canMove = status;
    }

    public void ToggleVulnerability(bool status)
    {
        isVulnerable = status;

        int alpha = status ? 255 : 100;

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color32(255, 255, 255, (byte)alpha);
        }
    }

    public IEnumerator EnableTemporaryInvulnerability(float time)
    {
        ToggleVulnerability(false);
        yield return new WaitForSeconds(time);
        ToggleVulnerability(true);
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
        } else if (canMove)
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

    public void Die()
    {
        if (!isVulnerable) return;

        AudioManager.instance.PlayDeathSound();
        if (SettingsManager.isVibrationEnabled)
        {
            Vibration.Vibrate(100);
        }

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.GameOver();
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
        if(canMove)
        {
            rb.linearVelocity = new Vector2(0, directionY * playerSpeed);
        }
    }
}
