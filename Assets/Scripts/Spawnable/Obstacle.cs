using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : AbstractSpawnable
{
    private float rotationDirectionMultiplier = 0f;
    private int randomRotationSpeedMultiplier = 0;

    protected override void Awake()
    {
        base.Awake();

        myTransform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))); // Random Z rotation https://forum.unity.com/threads/instantiate-with-a-random-y-rotation.345052/
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        bool rotateBackwards = (Random.value > 0.5f);
        rotationDirectionMultiplier = (rotateBackwards ? (-1) : 1);
        randomRotationSpeedMultiplier = Random.Range(1, 80);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        myTransform.Rotate(0, 0, rotationDirectionMultiplier * Time.deltaTime * randomRotationSpeedMultiplier, Space.Self);
    }

    public override void NegateMovement()
    {
        base.NegateMovement();

        rotationDirectionMultiplier *= -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            AudioManager.instance.PlayDeathSound();
            if (SettingsManager.isVibrationEnabled)
            {
                Vibration.Vibrate(100);
            }

            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.GameOver();
        }

        if(collision.tag == "Border")
        {
            gameObject.SetActive(false);
        }
    }
}
