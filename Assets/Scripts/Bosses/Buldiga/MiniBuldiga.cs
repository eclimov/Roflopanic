using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniBuldiga : MonoBehaviour
{
    public Sprite spriteBad;
    public Sprite spriteGood;
    public GameObject bloodSplashPrefab;
    public AudioClip soundTouch;
    public AudioClip soundExplode;
    public AudioClip soundSplash;
    public AudioClip soundWarningBeep;

    private bool isGood;

    private bool isDying; // If true, do not react on collosions with player
    private bool isDead;

    private Vector3 moveDirection = Vector3.zero;

    private float speed;

    public void SetIsGood(bool status)
    {
        isGood = status;
        GetComponent<Image>().sprite = isGood ? spriteGood : spriteBad;
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }

    public void SetTargetDirection(Vector3 targetPosition)
    {
        Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position, transform.TransformDirection(Vector3.up));
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        moveDirection = (targetPosition - transform.position).normalized;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0) && !FindObjectOfType<PauseMenu>().isGamePaused)
        {
            if (isGood)
            {
                if (SettingsManager.isVibrationEnabled)
                {
                    Vibration.Vibrate(100);
                }
            }

            AudioManager.instance.PlaySound(soundTouch);
            TriggerDeath();
        }
    }

    public void TriggerDeath()
    {
        isDying = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            if(isGood)
            {
                FindObjectOfType<BuldigaBossGameManager>().AddBullet();
                Destroy(gameObject);
            } else if (!isDying)   
            {
                player.Die();
                TriggerDeath();
            }
        }
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * moveDirection;

        if (isGood) // Adding some shake to good minibuldigas
        {
            float angle = Mathf.PingPong(Time.time * 50, 10);
            transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        if (isDying)
        {
            if (transform.localScale.x < 1.3f)
            {
                transform.localScale += 3 * Time.deltaTime * Vector3.right;
                transform.localScale += 3 * Time.deltaTime * Vector3.up;
            } else if (!isDead) // Check the flag, to make sure we don't enter this scope twice
            {
                if (isGood)
                {
                    AudioManager.instance.PlaySound(soundWarningBeep);
                }

                AudioManager.instance.PlaySound(soundExplode);
                AudioManager.instance.PlaySound(soundSplash);

                GameObject bloodSplashGameObject = Instantiate(bloodSplashPrefab, GameObject.Find("Canvas UI").transform);
                bloodSplashGameObject.transform.position = gameObject.transform.position;

                isDead = true;
                Destroy(gameObject);
            }
        }
    }
}
