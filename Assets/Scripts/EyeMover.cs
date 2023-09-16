using UnityEngine;
using System.Collections;

public class EyeMover : MonoBehaviour
{
    [Tooltip("Pupil that will be moved in the direction of the target")]
    [SerializeField]
    private Transform eyes;

    public float maxMovementDistanceLeft = .1f;
    public float maxMovementDistanceUp = .1f;
    public float maxMovementDistanceRight = .1f;
    public float maxMovementDistanceDown = .1f;

    [Tooltip("Objects the eyes are looking at. Optional")]
    [SerializeField]
    private Transform[] lookTargets = new Transform[0]; // Optional

    private Transform lookAtTarget;

    private Vector3 movementDistance;

    private Vector3 direction;
    private Vector3 offset;
    private Transform myTransform;
    private Transform eyesTransform;

    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;

    private void Awake()
    {
        direction = Vector3.zero;
        movementDistance = Vector3.zero;

        // For Optimization purposes
        myTransform = transform;
        eyesTransform = eyes.transform;

        cachedWaitForSecondsRealtime = new WaitForSecondsRealtime(2f);
    }

    private void OnEnable()
    {
        if (lookAtTarget == null)
        {
            StartCoroutine(InfiniteTargetRefresh());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        if(lookAtTarget == null)
        {
            direction = Vector3.zero;
            movementDistance = Vector3.zero; // Reset eyes movement
        } else
        {
            direction = lookAtTarget.position - myTransform.position;

            // https://docs.unity3d.com/ScriptReference/Vector3.Index_operator.html
            movementDistance[0] = direction.normalized.x < 0 ? maxMovementDistanceLeft : maxMovementDistanceRight; // If the target is positioned left (relatively to the eyes)
            movementDistance[1] = direction.normalized.y < 0 ? maxMovementDistanceDown : maxMovementDistanceUp; // If the target is positioned bottom (relatively to the eyes)
        }

        offset = Vector3.Scale(direction.normalized, movementDistance);
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
