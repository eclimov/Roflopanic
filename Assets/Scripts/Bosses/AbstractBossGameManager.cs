using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBossGameManager : MonoBehaviour
{
    public GameObject bossHealthManagerPrefab;
    public GameObject bossVictoryTextPrefab;
    
    public AudioClip music;

    public Boss boss;

    protected GameManager gameManager;
    protected HealthManager bossHealthManager;

    protected ScoreManager scoreManager;

    protected string bossName = "boss name";
    protected float bossRewardScore = 1f;

    protected Player player;

    protected virtual void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        player = FindObjectOfType<Player>();

        bossHealthManager = Instantiate(bossHealthManagerPrefab, GameObject.Find("Canvas").transform).GetComponent<HealthManager>();

        gameManager.OnGameOver += OnGameOverHandler;
        gameManager.OnReincarnationStarted += OnReincarnationStartedHandler;
        gameManager.OnReincarnationEnded += OnReincarnationEndedHandler;
        bossHealthManager.OnHealthZero += OnBossHealthZeroHandler;
        boss.OnDamageTaken += OnDamageTakenHandler;
    }

    protected virtual void OnDestroy()
    {
        gameManager.OnGameOver -= OnGameOverHandler;
        gameManager.OnReincarnationStarted -= OnReincarnationStartedHandler;
        gameManager.OnReincarnationEnded -= OnReincarnationEndedHandler;
        bossHealthManager.OnHealthZero -= OnBossHealthZeroHandler;
        boss.OnDamageTaken -= OnDamageTakenHandler;
    }

    protected virtual void Start()
    {
        // Make player invulnerable temporary, to prevent from colliding with obstacles
        StartCoroutine(player.EnableTemporaryInvulnerability(2f)); // TODO: think on "WaitUntil" logic, if explicit wait is not enough for all difficulties
        AudioManager.instance.PlayMusic(music);
    }

    protected abstract void OnGameOverHandler();
    protected abstract void OnReincarnationStartedHandler();
    protected abstract void OnReincarnationEndedHandler();

    protected virtual IEnumerator BossFightWon()
    {
        boss.Die();

        yield return new WaitForSecondsRealtime(1f); // A delay between death animation and victory text

        AudioManager.instance.PauseMusic();

        TitleFadeText bossVictoryText = Instantiate(bossVictoryTextPrefab, GameObject.Find("Canvas").transform).GetComponent<TitleFadeText>();
        bossVictoryText.SetText(bossName, TitleFadeText.TitleTextType.Boss);
        bossVictoryText.FadeIn();

        yield return new WaitForSecondsRealtime(3f); // Wait for fadeIn to finish and a little bit more

        Destroy(bossHealthManager.gameObject); // Destroy boss health bar (because it's not a child of the current gameobject)

        bossVictoryText.FadeOut();
        yield return new WaitForSecondsRealtime(1f); // Wait for fadeOut to finish

        AudioManager.instance.PlayCashSound();
        scoreManager.AddBonusScore(bossRewardScore);

        gameManager.EndBossFight(gameObject); // This should go the last, because boss game manager is destroyed there
    }

    protected abstract void IncrementBossFightWinCount();

    protected void OnBossHealthZeroHandler()
    {
        IncrementBossFightWinCount();
        StartCoroutine(BossFightWon());
    }

    protected void OnDamageTakenHandler(int damage)
    {
        boss.PlayDamageAnimation();
        bossHealthManager.TakeDamage(damage);
    }

    public void SetBossName(string name)
    {
        bossName = name;
        bossHealthManager.SetName(bossName);
    }

    public void BossFightLost()
    {
        gameManager.GameOver();
    }

    public void SetRewardScore(int reward)
    {
        bossRewardScore = reward;
    }

    public virtual void MakeBossInvulnerable()
    {
        boss.MakeInvulnerable();
        bossHealthManager.EnableShieldLayer();
    }

    public virtual void MakeBossVulnerable()
    {
        boss.MakeVulnerable();
        bossHealthManager.DisableShieldLayer();
    }
}
