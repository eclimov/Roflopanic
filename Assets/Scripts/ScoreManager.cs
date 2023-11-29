using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [System.Serializable]
    public class BossFightConfig
    {
        public int rewardScore;
        public Sprite characterImage;
        public float triggerScore;
        public Dialogue dialogue;
        public AudioClip bossVoiceLetterSound;
        public AbstractBossGameManager bossGameManager;
    }

    public Text scoreText;

    public Animator scoreTextAnimator;

    public float score = 0f;
    private float targetScore = 0;

    public GameObject floatingScoreTextPrefab;

    public List<BossFightConfig> bossFightConfigs;
    public GameObject bossFightConfirmPanelPrefab;

    private bool isTransitioning;

    private float coinBonusScore;

    private float defaultIncrementMultiplier;
    private float incrementMultiplier;

    private GameManager gameManager;

    private WaitForSeconds cachedWaitForSecondsBeforeIncrement;

    private LevelLoader levelLoader;
    private bool isLevelLoaded;

    private GameObject playerGameObject;

    private Queue<BossFightConfig> bossFightConfigsQueue = new Queue<BossFightConfig>();
    private BossFightConfig nearestBossFightTriggerConfig;
    private float oldScore;

    private bool isContinuousScoreIncreaseAllowed = true;

    private PauseMenu pauseMenu;

    private void Start()
    {
        cachedWaitForSecondsBeforeIncrement = new WaitForSeconds(1f); // Use scaled time here, to avoid incrementing score during pause
        defaultIncrementMultiplier = SettingsManager.instance.GetDifficultyMap().scoreIncrementMultiplier;
        coinBonusScore = SettingsManager.instance.GetDifficultyMap().coinBonusScore;
        playerGameObject = FindObjectOfType<Player>().gameObject;

        pauseMenu = FindObjectOfType<PauseMenu>();

        foreach (BossFightConfig bossFightConfig in bossFightConfigs) { // Filling Queue for a more optimal work in Update
            bossFightConfigsQueue.Enqueue(bossFightConfig);
        }
        nearestBossFightTriggerConfig = bossFightConfigsQueue.Dequeue(); // Initialize the 1st one

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
        if (score < targetScore)
        {
            if(score > nearestBossFightTriggerConfig.triggerScore && bossFightConfigsQueue.Count > 0)
            {
                nearestBossFightTriggerConfig = bossFightConfigsQueue.Dequeue();
            }
            oldScore = score;

            score += incrementMultiplier * Time.deltaTime;

            if (oldScore < nearestBossFightTriggerConfig.triggerScore && score >= nearestBossFightTriggerConfig.triggerScore)
            {
                BossFightConfirmManager bossFightConfirmManager = Instantiate(bossFightConfirmPanelPrefab, GameObject.Find("Canvas UI").transform, false)
                    .GetComponent<BossFightConfirmManager>();
                bossFightConfirmManager.SetBossImage(nearestBossFightTriggerConfig.characterImage);
                bossFightConfirmManager.StartDialogue(nearestBossFightTriggerConfig.dialogue);
                bossFightConfirmManager.SetRewardValue(nearestBossFightTriggerConfig.rewardScore);
                bossFightConfirmManager.SetBossGameManager(nearestBossFightTriggerConfig.bossGameManager);
                bossFightConfirmManager.SetBossVoiceLetterSound(nearestBossFightTriggerConfig.bossVoiceLetterSound);
            }

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
        return isLevelLoaded 
            && !(GameManager.isGameOver || pauseMenu.isGamePaused);
    }

    public void AllowContinuousIncrease(bool status)
    {
        isContinuousScoreIncreaseAllowed = status;
    }

    public void IncreaseTargetScore(float value = 1f)
    {
        if (CanIncreaseScore() && !isTransitioning && isContinuousScoreIncreaseAllowed)
        {
            targetScore += value;
            isTransitioning = true;
        }
    }

    public void AddBonusScore(float bonusScore)
    {
        targetScore += bonusScore;
        isTransitioning = true;
        incrementMultiplier = 70;

        scoreTextAnimator.SetBool("IsBonus", true);

        // Show floating text animation
        GameObject floatingScoreText = Instantiate(floatingScoreTextPrefab, playerGameObject.transform.position + Vector3.up, Quaternion.identity);
        floatingScoreText.GetComponentInChildren<TextMesh>().text = "+" + bonusScore.ToString();
    }

    public void OnCoinCollected()
    {
        AddBonusScore(coinBonusScore);
    }
}
