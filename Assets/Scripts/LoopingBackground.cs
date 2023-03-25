using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float defaultBackgroundSpeed;
    public Renderer backgroundRenderer;

    private Vector2 scrolling;

    private static LoopingBackground instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        float backgroundSpeed = defaultBackgroundSpeed;
        if (scene.name == "Gameplay")
        {
            backgroundSpeed = 20 * defaultBackgroundSpeed;
        }

        scrolling = new Vector2(backgroundSpeed, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        backgroundRenderer.material.mainTextureOffset += scrolling * Time.deltaTime;
    }
}
