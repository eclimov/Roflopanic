using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public abstract class AbstrctBackground : MonoBehaviour
{
    public Renderer backgroundRenderer;

    protected Vector2 scrolling;

    private void Awake()
    {
        float oldWidth = backgroundRenderer.bounds.size.x;
        float oldHeight = backgroundRenderer.bounds.size.y;

        // Scale to screen size
        var newHeight = (float)(Camera.main.orthographicSize * 2.0);
        var newWidth = (float)(newHeight * Screen.width / Screen.height);
        transform.localScale = new Vector3(newWidth, newHeight, 1);

        // Tile texture to scaled size
        backgroundRenderer.material.mainTextureScale = new Vector2(newWidth / oldWidth, newHeight / oldHeight);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        SetScrollingSpeed();
    }

    protected abstract void SetScrollingSpeed();

    // Update is called once per frame
    void Update()
    {
        backgroundRenderer.material.mainTextureOffset += scrolling * Time.deltaTime;
    }
}
