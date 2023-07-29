using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMovement : MonoBehaviour
{
    public float movementSpeed = 0f;
    public float rotationSpeed = 0f;

    void Update()
    {
        transform.position -= new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        transform.Rotate(0, 0, -Time.deltaTime * rotationSpeed, Space.Self);
    }
}
