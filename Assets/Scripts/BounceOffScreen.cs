using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOffScreen : MonoBehaviour
{
    private float speedX;
    private float speedY;

    private float transformMultiplierX = 1f;
    private float transformMultiplierY = 1f;

    void Awake()
    {
        speedX = Random.Range(1f, 10f);
        speedY = Random.Range(1f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if ((screenPosition.x > Screen.width) || (screenPosition.x < 0f))
        {
            if(Mathf.Sign(screenPosition.x) == Mathf.Sign(transformMultiplierX)) // Do not change direction until position normalizes
            {
                transformMultiplierX = -transformMultiplierX;
            }
        }

        if((screenPosition.y > Screen.height) || (screenPosition.y < 0f))
        {
            if (Mathf.Sign(screenPosition.y) == Mathf.Sign(transformMultiplierY)) // Do not change direction until position normalizes
            {
                transformMultiplierY = -transformMultiplierY;
            }
        }

        transform.position += new Vector3(speedX * Time.deltaTime * transformMultiplierX, speedY * Time.deltaTime * transformMultiplierY, 0);
    }
}
