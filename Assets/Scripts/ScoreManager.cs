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

    public GameObject floatingScoreTextPrefab;

    private bool isTransitioning;

    private float coinBonusScore;

    private float defaultIncrementMultiplier;
    private float incrementMultiplier;

    private GameManager gameManager;

    private WaitForSeconds cachedWaitForSecondsBeforeIncrement;

    private LevelLoader levelLoader;
    private bool isLevelLoaded;

    private void Start()
    {
        cachedWaitForSecondsBeforeIncrement = new WaitForSeconds(1f);
        defaultIncrementMultiplier = SettingsManager.instance.GetDifficultyMap().scoreIncrementMultiplier;
        coinBonusScore = SettingsManager.instance.GetDifficultyMap().coinBonusScore;

        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnGameOver += OnGameOverHandler;

        ResetIncrementMultiplier();
        SetTextValue();

        levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.OnLevelLoad += OnLevelLoadHandler;
    }

    protected void OnDestroy()
    {
        levelLoader.OnLevelLoad -= OnLevelLoadHandler;
        gameManager.OnGameOver -= OnGameOverHandler;
    }

    private void OnLevelLoadHandler()
    {
        isLevelLoaded = true;
        StartCoroutine(ScoreIncrementTimer());
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

        int scoreToAdd = GetScore();
        SettingsManager.instance.AddScore(scoreToAdd);
        SettingsManager.instance.AddExperience(scoreToAdd);
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
        return isLevelLoaded && !(GameManager.isGameOver || PauseMenu.GameIsPaused);
    }

    public void IncreaseTargetScore(float value = 1f)
    {
        if (CanIncreaseScore() && !isTransitioning)
        {
            targetScore += value;
            isTransitioning = true;
        }
    }

    public void OnCoinCollected(GameObject playerGameObject)
    {
        targetScore += coinBonusScore;
        isTransitioning = true;
        incrementMultiplier = 70;

        scoreTextAnimator.SetBool("IsBonus", true);

        // Show floating text animation
        GameObject floatingScoreText = Instantiate(floatingScoreTextPrefab, playerGameObject.transform.position + Vector3.up, Quaternion.identity);
        floatingScoreText.GetComponentInChildren<TextMesh>().text = "+" + coinBonusScore.ToString();
    }
}
