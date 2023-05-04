using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpPanelHandler : MonoBehaviour
{
    public delegate void OnPanelCloseDelegate();
    public event OnPanelCloseDelegate OnPanelClose;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        AudioManager.instance.PauseMusic();
    }

    public void ClosePanel()
    {
        if (OnPanelClose != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnPanelClose();
        }

        Time.timeScale = 1f;
        AudioManager.instance.UnpauseMusic();
        Destroy(gameObject);
    }
}
