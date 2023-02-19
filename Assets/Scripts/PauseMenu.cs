using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    // Reset state before destroying the object
    void OnDestroy()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Resume ()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause ()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // FYI: how to animate during pause - https://www.youtube.com/watch?v=82MgDp8b_ms
        GameIsPaused = true;
    }
}
