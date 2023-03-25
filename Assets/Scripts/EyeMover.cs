using UnityEngine;
using System.Collections;

public class EyeMover : MonoBehaviour
{
    [Tooltip("Pupil that will be moved in the direction of the target")]
    [SerializeField]
    private Transform eyes;
    [Tooltip("The distance the pupil is alowed to travel from the center of the eye.")]
    [SerializeField]
    private float movementDistance = 0.1f;

    [Tooltip("Object the eyes are looking at. Optional")]
    [SerializeField]
    private Transform lookAtTarget; // Optional

    private Vector3 direction;
    private Vector3 offset;
    private Transform myTransform;
    private Transform eyesTransform;

    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;

    private void Awake()
    {
        direction = Vector3.zero;

        // For Optimization purposes
        myTransform = transform;
        eyesTransform = eyes.transform;

        cachedWaitForSecondsRealtime = new WaitForSecondsRealtime(2f);
    }

    private void Start()
    {
        if (lookAtTarget == null)
        {
            StartCoroutine(InfiniteTargetRefresh());
        }
    }

    void Update()
    {
        if(lookAtTarget == null)
        {
            direction = Vector3.zero;
        } else
        {
            direction = lookAtTarget.position - myTransform.position;
        }

        offset = direction.normalized * movementDistance;
        eyesTransform.localPosition = Vector3.Lerp(eyesTransform.localPosition, new Vector3(0f, .25f) + offset, Time.unscaledDeltaTime* 20f); // Smooth translate
    }

    IEnumerator InfiniteTargetRefresh()
    {
        for (; ; )
        {
            if(Random.Range(0, 2) == 1)
            {
                SetRandomObstacleAsTarget();
            } else
            {
                lookAtTarget = null;
            }
            yield return cachedWaitForSecondsRealtime;
        }
    }

    void SetRandomObstacleAsTarget()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        if (obstacles.Length != 0)
        {
            lookAtTarget = obstacles[Random.Range(0, obstacles.Length)].transform; // Look at a random enemy
        }
    }
}