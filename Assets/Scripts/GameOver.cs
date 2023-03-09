using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static bool isGameOver;
    public GameObject gameOverPanel;

    private void Awake()
    {
        isGameOver = false; // Leave it here, to reset the value on restart
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver && !gameOverPanel.activeInHierarchy)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
