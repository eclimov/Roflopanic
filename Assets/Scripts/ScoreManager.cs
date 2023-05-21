using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;

    public Animator scoreTextAnimator;

    public float score = 0f;
    private float targetScore = 0;

    private bool isTransitioning;

    private float coinBonusScore;

    private float defaultIncrementMultiplier;
    private float incrementMultiplier;

    private GameManager gameManager;

    private WaitForSeconds cachedWaitForSecondsBeforeIncrement;

    private void Start()
    {
        cachedWaitForSecondsBeforeIncrement = new WaitForSeconds(1f);
        defaultIncrementMultiplier = SettingsManager.instance.GetDifficultyMap().scoreIncrementMultiplier;
        coinBonusScore = SettingsManager.instance.GetDifficultyMap().coinBonusScore;

        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnGameOver += OnGameOverHandler;

        ResetIncrementMultiplier();
        SetTextValue();
        StartCoroutine(ScoreIncrementTimer());
    }

    protected void OnDestroy()
    {
        gameManager.OnGameOver -= OnGameOverHandler;
    }

    private void OnGameOverHandler()
    {
        StartCoroutine(UpdateScoreWithADelay(.1f)); // we need a delay here, because ProgressBar doesn't react on the score's update otherwise
    }

    IEnumerator UpdateScoreWithADelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (IsHighscore())
        {
            SettingsManager.instance.SetHighscore(GetScore());
        }

        SettingsManager.instance.AddScore(GetScore());
    }

    public bool IsHighscore()
    {
        return GetScore() > SettingsManager.SaveData.highscore;
    }

    private void ResetIncrementMultiplier()
    {
        incrementMultiplier = defaultIncrementMultiplier;
    }

    private IEnumerator ScoreIncrementTimer()
    {
        while (true)
        {
            yield return cachedWaitForSecondsBeforeIncrement;
            IncreaseTargetScore();
        }
    }

    void Update()
    {
        if (CanIncreaseScore())
        {
            if (score < targetScore)
            {
                score += incrementMultiplier * Time.deltaTime;
                SetTextValue();
            } else if(isTransitioning)
            {
                isTransitioning = false;
                ResetIncrementMultiplier();

                if (scoreTextAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScoreText_Bonus"))
                {
                    scoreTextAnimator.SetBool("IsBonus", false);
                }
            }
        }
    }

    public int GetScore()
    {
        return (int)score;
    }

    private void SetTextValue()
    {
        scoreText.text = GetScore().ToString();
    }

    private bool CanIncreaseScore()
    {
        return !(GameManager.isGameOver || PauseMenu.GameIsPaused);
    }

    public void IncreaseTargetScore(float value = 1f)
    {
        if (CanIncreaseScore() && !isTransitioning)
        {
            targetScore += value;
            isTransitioning = true;
        }
    }

    public void OnCoinCollected()
    {
        targetScore += coinBonusScore;
        isTransitioning = true;
        incrementMultiplier = 70;

        scoreTextAnimator.SetBool("IsBonus", true);
    }
}
