using UnityEngine;
using System.Collections;

public class EyeMover : MonoBehaviour
{
    [Tooltip("Pupil that will be moved in the direction of the target")]
    [SerializeField]
    private Transform eyes;

    [Tooltip("Objects the eyes are looking at. Optional")]
    [SerializeField]
    private Transform[] lookTargets = new Transform[0]; // Optional

    private Transform lookAtTarget;

    [Tooltip("The default distance the pupil is alowed to travel from the center of the eye.")]
    private float movementDistance = .1f;

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

            // If the target is positioned bottom (relatively to the eyes) - move pupils nearer
            if (direction.normalized.y < 0)
            {
                movementDistance = .08f;
            }
        }

        offset = direction.normalized * movementDistance;
        eyesTransform.localPosition = Vector3.Lerp(eyesTransform.localPosition, new Vector3(0f, 0f) + offset, Time.unscaledDeltaTime * 20f); // Smooth translate
    }

    IEnumerator InfiniteTargetRefresh()
    {
        for (; ; )
        {
            if(lookTargets.Length > 0)
            {
                SetRandomObjectFromTheListAsTarget();
            }
            else if(Random.Range(0, 2) == 1)
            {
                SetRandomObstacleAsTarget();
            } else
            {
                lookAtTarget = null;
            }
            yield return cachedWaitForSecondsRealtime;
        }
    }

    void SetRandomObjectFromTheListAsTarget()
    {
        lookAtTarget = lookTargets[Random.Range(0, lookTargets.Length)].transform; // Look at a random game object from the list
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
