using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectComponent;
    private float rotateSpeed = -200f; // Negative value to make it rotate clockwise

    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectComponent.Rotate(0f, 0f, rotateSpeed * Time.unscaledDeltaTime);
    }
}