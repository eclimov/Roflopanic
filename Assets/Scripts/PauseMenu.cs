using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool isGamePaused;

    public GameObject pauseMenuUI;
    public GameObject timeFreezeOverlayPanel;

    // Reset (unfreeze + un-slowmotion) before destroying the object
    private void OnDestroy()
    {
        FreezeTime(false);
    }

    private void FreezeTime(bool status)
    {
        timeFreezeOverlayPanel.SetActive(status);
        pauseMenuUI.SetActive(status);

        float timeScale = status ? 0f : 1f;
        Time.timeScale = timeScale; // FYI: how to animate during pause - https://www.youtube.com/watch?v=82MgDp8b_ms

        isGamePaused = status;

        AudioManager.instance.SetSlowMotion(status);
    }

    public void Resume ()
    {
        FreezeTime(false);
    }

    public void Pause ()
    {
        FreezeTime(true);
    }
}
