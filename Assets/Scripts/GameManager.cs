using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver;
    public GameObject gameOverPanel;

    public GameObject timeFreezeOverlayPanel;

    public ObstacleSpawner obstacleSpawner;
    public GameObject rightBorder;

    [HideInInspector]
    public bool isBossFight;

    public AudioClip reincarnationSound;

    public delegate void OnGameOverDelegate();
    public event OnGameOverDelegate OnGameOver;

    public delegate void OnReincarnationStartedDelegate();
    public event OnReincarnationStartedDelegate OnReincarnationStarted;

    public delegate void OnReincarnationEndedDelegate();
    public event OnReincarnationEndedDelegate OnReincarnationEnded;

    private GameObject playerGameObject;
    private bool canReincarnate;

    private ScoreManager scoreManager;

    private void Awake()
    {
        isGameOver = false; // Leave it here, to reset the value on restart
    }

    private void Start()
    {
        playerGameObject = GameObject.FindGameObjectWithTag("Player").gameObject; // Make sure Player object is instantiated by this time
        canReincarnate = SettingsManager.IsAbilityEquipped("reincarnation");

        scoreManager = FindObjectOfType<ScoreManager>();
    }

    public void StartBossFight(AbstractBossGameManager bossGameManagerPrefab, string bossName, int reward)
    {
        AbstractBossGameManager bossGameManager = Instantiate(bossGameManagerPrefab, GameObject.Find("Canvas").transform, false);
        bossGameManager.SetBossName(bossName);
        bossGameManager.SetRewardScore(reward);

        scoreManager.AllowContinuousIncrease(false); // Temporary stop increasing score (except for coins)

        isBossFight = true;
        StopSpawn();
    }

    public void EndBossFight(GameObject bossGameManager)
    {
        Destroy(bossGameManager);

        scoreManager.AllowContinuousIncrease(true); // Allow increasing score

        isBossFight = false;
        ResumeSpawn();

        AudioManager.instance.PlayMusic("music-gameplay");
    }

    public void StopSpawn()
    {
        obstacleSpawner.StopSpawn(); // Stop spawning new items
    }

    public void ResumeSpawn()
    {
        obstacleSpawner.StartSpawn(); // Resume spawning new items
    }

    private IEnumerator Reincarnate()
    {
        if (OnReincarnationStarted != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnReincarnationStarted();
        }

        BackgroundScroller backgroundScroller = FindObjectOfType<BackgroundScroller>();
        backgroundScroller.NegateScrolling();

        StopSpawn();

        AbstractSpawnable[] spawnables = FindObjectsOfType<AbstractSpawnable>();

        foreach (AbstractSpawnable obj in spawnables) // Move all spawnables in the opposite directon
        {
            obj.NegateMovement();
        }

        AudioManager.instance.PauseMusic();
        AudioManager.instance.PlaySound(reincarnationSound);

        timeFreezeOverlayPanel.SetActive(true);

        if(!isBossFight && spawnables.Length > 0)
        {
            rightBorder.SetActive(true); // Temporary enable right border, to cleanup obstacles
            yield return new WaitUntil(() => Array.TrueForAll(spawnables, spawnable => spawnable.gameObject.activeSelf == false)); // Wait until all spawnables are disabled
            rightBorder.SetActive(false); // Disable right border, to prevent obstacles from unexpected intersection with it
        } else
        {
            yield return new WaitForSecondsRealtime(2f);
        }

        timeFreezeOverlayPanel.GetComponent<TimeFreezeOverlayPanel>().FadeOut();

        yield return new WaitForSecondsRealtime(1f);

        AudioManager.instance.StopPlayingSounds();
        AudioManager.instance.PlayButtonLipsSound();
        AudioManager.instance.UnpauseMusic();

        if (OnReincarnationEnded != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnReincarnationEnded();
        }

        // Idea: rework the following to use OnReincarnationStarted and OnReincarnationEnded events

        playerGameObject.SetActive(true);
        playerGameObject.GetComponent<Player>().OnReincarnate();
        backgroundScroller.NegateScrolling();

        foreach (AbstractSpawnable obj in spawnables) // Move all spawnables in the correct directon
        {
            obj.NegateMovement();
            obj.gameObject.SetActive(false); // Disable all spawnables to avoid overlaying 
        }

        yield return new WaitForSecondsRealtime(1f);

        canReincarnate = false; // Cannot reincarnate anymore during current game
        if(!isBossFight)
        {
            ResumeSpawn(); // Allow spawning obstacles and coins
        }
        isGameOver = false; // This flag is important for gaining score. Allow gaining score
    }

    public void GameOver()
    {
        playerGameObject.SetActive(false);
        isGameOver = true;

        if (canReincarnate)
        {
            StartCoroutine(Reincarnate());
            return;
        }

        gameOverPanel.SetActive(true);

        AudioManager.instance.StopMusic();

        if (OnGameOver != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnGameOver();
        }
    }
}
