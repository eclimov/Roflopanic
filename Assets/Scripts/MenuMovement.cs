using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMovement : MonoBehaviour
{
    //Rotational Speed
    public float speed = 0f;

    void Update()
    {
        transform.Rotate(0, 0, -Time.deltaTime * speed, Space.Self);
        transform.position -= new Vector3(speed * 3 * Time.deltaTime, 0, 0);
    }
}
