using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float defaultBackgroundSpeed;
    public Renderer backgroundRenderer;

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

    // Update is called once per frame
    void Update()
    {
        float backgroundSpeed = defaultBackgroundSpeed;
        if (GameObject.FindGameObjectWithTag("MenuPanel") == null)
        {
            backgroundSpeed = 20 * defaultBackgroundSpeed;
        }

        backgroundRenderer.material.mainTextureOffset += new Vector2(backgroundSpeed * Time.deltaTime, 0f);
    }
}
