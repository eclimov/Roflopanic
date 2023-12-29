using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    public float amplitude = 10;

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.PingPong(Time.time * 80, amplitude) - amplitude/2;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
