using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static bool isGameOver;
    public static bool isHighscore;
    public static bool isEligibleForReward;
    public GameObject gameOverPanel;

    private void Awake()
    {
        isGameOver = false; // Leave it here, to reset the value on restart
        isHighscore = false;
        isEligibleForReward = true;
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

            // Order of operations below is important, to allow animating progress bar from former total score, with a delay
            gameOverPanel.SetActive(true);
            settingsManager.AddScore(score);
            StartCoroutine(AnimateProgressBarWithADelay(SettingsManager.totalScore, 1f));
        }
    }

    IEnumerator AnimateProgressBarWithADelay(int to, float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<ProgressBar>().AnimateProgress(to, .1f);
    }
}
