using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector3 speedVector;

    private Transform myTransform;

    private void Awake()
    {
        // For Optimization purposes
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        speedVector = new Vector3(SettingsManager.speed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.position -= speedVector * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            gameObject.SetActive(false);
        }
    }
}
