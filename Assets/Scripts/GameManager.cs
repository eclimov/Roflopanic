using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver;
    public GameObject gameOverPanel;

    public delegate void OnGameOverDelegate();
    public event OnGameOverDelegate OnGameOver;

    private void Awake()
    {
        isGameOver = false; // Leave it here, to reset the value on restart
    }

    public void SetGameOver(bool status)
    {
        isGameOver = status;
        gameOverPanel.SetActive(true);

        if (OnGameOver != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnGameOver();
        }
    }
}
