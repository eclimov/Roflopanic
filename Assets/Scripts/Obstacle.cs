using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float rotationDirectionMultiplier = 0f;
    private int randomRotationSpeedMultiplier = 0;
    public float speed;

    private Transform myTransform;

    private void Awake()
    {
        // For Optimization purposes
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        bool rotateBackwards = (Random.value > 0.5f);
        rotationDirectionMultiplier = (rotateBackwards ? (-1) : 1);
        randomRotationSpeedMultiplier = Random.Range(1, 80);
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.Rotate(0, 0, rotationDirectionMultiplier * Time.deltaTime * randomRotationSpeedMultiplier, Space.Self);
        myTransform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Border")
        {
            Destroy(this.gameObject);
        }

        else if(collision.tag == "Player")
        {
            AudioManager.instance.PlayDeathSound();
            Destroy(GameObject.FindGameObjectWithTag("Player").gameObject);

            if (SettingsManager.isVibrationEnabled)
            {
                Vibration.Vibrate(100);
            }
        }
    }
}
