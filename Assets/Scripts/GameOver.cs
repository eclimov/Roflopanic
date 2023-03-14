using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static bool isGameOver;
    public static bool isHighscore;
    public GameObject gameOverPanel;

    private void Awake()
    {
        isGameOver = false; // Leave it here, to reset the value on restart
        isHighscore = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver && !gameOverPanel.activeInHierarchy)
        {
            SettingsManager settingsManager = FindObjectOfType<SettingsManager>();

            int score = FindObjectOfType<ScoreManager>().GetScore();
            if (score > SettingsManager.highscore)
            {
                isHighscore = true;
                settingsManager.SetHighscore(score);
            }
            settingsManager.AddScore(score);

            gameOverPanel.SetActive(true);
        }
    }
}
