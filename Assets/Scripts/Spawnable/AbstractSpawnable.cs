using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpawnable : MonoBehaviour
{
    public Vector3 speedVector;
    public Transform myTransform;

    protected virtual void Awake()
    {
        // For Optimization purposes
        myTransform = transform;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        speedVector = new Vector3(SettingsManager.instance.GetDifficultyMap().obstacleSpeed, 0, 0);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        myTransform.position -= speedVector * Time.deltaTime;
    }

    public virtual void NegateMovement()
    {
        speedVector *= -1;
    }
}
