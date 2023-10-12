using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBossGameManager : MonoBehaviour
{
    public GameObject bossHealthManagerPrefab;
    public GameObject bossVictoryTextPrefab;
    
    public AudioClip music;

    protected Boss boss;

    protected GameManager gameManager;
    protected BossHealthManager bossHealthManager;

    protected ScoreManager scoreManager;

    protected string bossName = "boss name";
    protected float bossRewardScore = 1f;

    protected virtual void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();

        boss = FindObjectOfType<Boss>();

        bossHealthManager = Instantiate(bossHealthManagerPrefab, GameObject.Find("Canvas").transform).GetComponent<BossHealthManager>();

        gameManager.OnGameOver += OnGameOverHandler;
        gameManager.OnReincarnationStarted += OnReincarnationStartedHandler;
        gameManager.OnReincarnationEnded += OnReincarnationEndedHandler;
        bossHealthManager.OnBossHealthZero += OnBossHealthZeroHandler;
        boss.OnDamageTaken += OnDamageTakenHandler;
    }

    protected void OnDestroy()
    {
        gameManager.OnGameOver -= OnGameOverHandler;
        gameManager.OnReincarnationStarted -= OnReincarnationStartedHandler;
        gameManager.OnReincarnationEnded -= OnReincarnationEndedHandler;
        bossHealthManager.OnBossHealthZero -= OnBossHealthZeroHandler;
        boss.OnDamageTaken -= OnDamageTakenHandler;
    }

    protected void Start()
    {
        AudioManager.instance.PlayMusic(music);
    }

    protected abstract void OnGameOverHandler();
    protected abstract void OnReincarnationStartedHandler();
    protected abstract void OnReincarnationEndedHandler();

    protected abstract IEnumerator BossFightWon();

    protected abstract void IncrementBossFightWinCount();

    protected void OnBossHealthZeroHandler()
    {
        IncrementBossFightWinCount();
        StartCoroutine(BossFightWon());
    }

    protected void OnDamageTakenHandler(int damage)
    {
        bossHealthManager.TakeDamage(damage);
    }

    public void SetBossName(string name)
    {
        bossName = name;
        bossHealthManager.SetBossName(bossName);
    }

    protected void BossFightLost()
    {
        gameManager.GameOver();
    }

    public void SetRewardScore(int reward)
    {
        bossRewardScore = reward;
    }

    public virtual void MakeBossVulnerable()
    {
        boss.MakeVulnerable();
        bossHealthManager.DisableShieldLayer();
    }
}
