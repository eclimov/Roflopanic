using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float rotationDirectionMultiplier = 0f;
    private int randomRotationSpeedMultiplier = 0;

    private Vector3 speedVector;

    private Transform myTransform;

    private void Awake()
    {
        // For Optimization purposes
        myTransform = transform;

        myTransform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))); // Random Z rotation https://forum.unity.com/threads/instantiate-with-a-random-y-rotation.345052/
    }

    // Start is called before the first frame update
    void Start()
    {
        bool rotateBackwards = (Random.value > 0.5f);
        rotationDirectionMultiplier = (rotateBackwards ? (-1) : 1);
        randomRotationSpeedMultiplier = Random.Range(1, 80);

        speedVector = new Vector3(SettingsManager.instance.GetDifficultyMap().obstacleSpeed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.Rotate(0, 0, rotationDirectionMultiplier * Time.deltaTime * randomRotationSpeedMultiplier, Space.Self);
        myTransform.position -= speedVector * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            AudioManager.instance.PlayDeathSound();
            Destroy(GameObject.FindGameObjectWithTag("Player").gameObject);
            FindObjectOfType<GameManager>().SetGameOver(true);

            if (SettingsManager.isVibrationEnabled)
            {
                Vibration.Vibrate(100);
            }
        }
    }
}
