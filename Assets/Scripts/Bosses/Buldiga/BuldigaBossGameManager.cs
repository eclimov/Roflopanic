using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuldigaBossGameManager : AbstractBossGameManager
{
    public static string bossFightsWonCountKey = "bossFightsWonBuldiga";

    public CountdownTimer countdownTimer;
    public GameObject miniBuldigaBulletPrefab;
    public List<GameObject> uiBulletGameObjects; // Make sure the order is natural (1, 2, 3, etc.)
    public AudioClip shootBulletSound;
    public AudioClip bulletLoadSound;
    public AudioClip reloadSound;

    public MiniBuldigaSpawnPoint miniBuldigaSpawnPointLeft;
    public MiniBuldigaSpawnPoint miniBuldigaSpawnPointRight;

    private Vector3 mousePos;
    private Camera mainCam;

    private float shootTime;
    private float timeBetweenShoot = .2f;

    private bool isReadyToShoot;

    protected override void Awake()
    {
        base.Awake();

        mainCam = Camera.main;

        FindObjectOfType<Player>().ToggleMovement(false);

        countdownTimer.OnTick += OnTimerTick;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        countdownTimer.OnTick -= OnTimerTick;
    }

    private void OnTimerTick(int timeRemains)
    {
        if(timeRemains == 0 && !boss.IsDead() && HasBullets())
        {
            TriggerBossUltimate();
        }
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(StartPhaseOneDelayed(1.2f));
    }

    private void StartPhaseOne()
    {
        StartCoroutine(SetIsReadyToShootDelayed(false, 0f));

        StartCoroutine(player.EnableTemporaryInvulnerability(1f));

        MakeBossInvulnerable();

        FindObjectOfType<PlayerManager>().TeleportPlayerToPositionCenter();
        ResumeMiniBuldigaSpawn();
    }

    private IEnumerator SetIsReadyToShootDelayed(bool status, float delay)
    {
        yield return new WaitForSeconds(delay);
        isReadyToShoot = status;
    }

    private IEnumerator StartPhaseOneDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartPhaseOne();
    }

    private void StartPhaseTwo()
    {
        StartCoroutine(player.EnableTemporaryInvulnerability(1f));

        boss.gameObject.SetActive(true);
        StartCoroutine(TriggerReloadSoundDelayed(1.1f));
        StartCoroutine(TriggerCountdownTimerDelayed(1.3f));
        StartCoroutine(SetIsReadyToShootDelayed(true, 1.3f));

        MakeBossVulnerable();
        PauseMiniBuldigaSpawnAndDestroyAllMiniBuldiga();
        FindObjectOfType<PlayerManager>().TeleportPlayerToPositionDefault();
    }

    private IEnumerator TriggerReloadSoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.PlaySound(reloadSound);
    }

    private IEnumerator TriggerCountdownTimerDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        float countdownSeconds;
        switch (SettingsManager.difficultyId)
        {
            case 1: // Medium
                countdownSeconds = 6f;
                break;
            case 2: // Hardcore
                countdownSeconds = 5f;
                break;
            default: // Easy
                countdownSeconds = 7f;
                break;
        }
        countdownTimer.SetCountdownTime(countdownSeconds);
    }

    private IEnumerator SpawnEvilMiniBuldigaDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        miniBuldigaSpawnPointRight.SpawnMiniBuldiga(10, false);
    }

    private void TriggerBossUltimate()
    {
        for (int i = 0; i < 10; i++)
        {
            StartCoroutine(SpawnEvilMiniBuldigaDelayed(Random.Range(0f, 2f)));
        }
    }

    public void AddBullet()
    {
        AudioManager.instance.PlaySound(bulletLoadSound);
        uiBulletGameObjects.Find(e => !e.activeSelf)?.SetActive(true);

        if (uiBulletGameObjects.TrueForAll(e => e.activeSelf) 
            && !boss.gameObject.activeSelf // And the boss is not activated yet (to prevent calling phase two twice)
        )
        {
            StartPhaseTwo();
        }
    }

    private IEnumerator ShootBullet(Vector2 direction)
    {
        if(HasBullets())
        {
            GameObject miniBuldigaBullet = Instantiate(miniBuldigaBulletPrefab, gameObject.transform);
            miniBuldigaBullet.transform.position = player.gameObject.transform.position;
            miniBuldigaBullet.GetComponent<MiniBuldigaBullet>().ThrowAt(direction);

            AudioManager.instance.PlaySound(shootBulletSound);

            uiBulletGameObjects.FindLast(e => e.activeSelf).SetActive(false);
            if (uiBulletGameObjects.TrueForAll(e => !e.activeSelf))
            {
                countdownTimer.Stop();

                yield return new WaitForSeconds(1.5f); // Wait until all bullets reach their target
                if(!boss.IsDead())
                {
                    boss.TriggerExit();
                }

                yield return new WaitForSeconds(1f);
                if (!boss.IsDead() && !GameManager.isGameOver)
                {
                    StartPhaseOne();
                }
            }
        }
    }

    private bool HasBullets()
    {
        return uiBulletGameObjects.Exists(e => e.activeSelf);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) // Same as touching the screen https://www.youtube.com/watch?v=0M-9EtUArhw
        {
            // the dimensions represent FOV: https://forum.unity.com/threads/screentoworldpoint-always-the-same.337105/
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (
                boss.gameObject.activeSelf // If it's phase two
                && !boss.IsDead() // disallow shootig more bullets when the boss is dead
                && Time.time > shootTime
                && !GameManager.isGameOver // Is not game over or is reincarnating
                && isReadyToShoot
            )
            {
                shootTime = Time.time + timeBetweenShoot;
                StartCoroutine(ShootBullet((mousePos - player.gameObject.transform.position).normalized));
            }
        }
    }

    protected override IEnumerator BossFightWon()
    {
        countdownTimer.Stop();

        // Destroy existing minibuldigas
        MiniBuldiga[] miniBuldigas = FindObjectsOfType<MiniBuldiga>();
        foreach (MiniBuldiga miniBuldiga in miniBuldigas)
        {
            StartCoroutine(DestroyMiniBuldigaWithDelay(miniBuldiga, Random.Range(0f, .2f)));
        }

        // Destroy existing mini buldiga bullets
        MiniBuldigaBullet[] miniBuldigaBullets = FindObjectsOfType<MiniBuldigaBullet>();
        foreach (MiniBuldigaBullet miniBuldigaBullet in miniBuldigaBullets)
        {
            miniBuldigaBullet.DestroyWithCircleExplosion();
        }

        FindObjectOfType<Player>().ToggleMovement(true);

        yield return base.BossFightWon();
    }

    protected override void IncrementBossFightWinCount()
    {
        SettingsManager.IncrementBossFightsWonCount(bossFightsWonCountKey);
    }

    protected override void OnGameOverHandler()
    {
        PauseMiniBuldigaSpawnAndDestroyAllMiniBuldiga();
    }

    protected override void OnReincarnationStartedHandler()
    {
        PauseMiniBuldigaSpawnAndDestroyAllMiniBuldiga();
    }

    protected override void OnReincarnationEndedHandler()
    {
        if(boss.gameObject.activeSelf) // If it's phase two
        {
            StartCoroutine(TriggerCountdownTimerDelayed(.5f));
        } else
        {
            StartPhaseOne();
        }
    }

    private void ResumeMiniBuldigaSpawn()
    {
        miniBuldigaSpawnPointLeft.ResumeSpawn();
        miniBuldigaSpawnPointRight.ResumeSpawn();
    }

    private void PauseMiniBuldigaSpawnAndDestroyAllMiniBuldiga()
    {
        miniBuldigaSpawnPointLeft.PauseSpawn();
        miniBuldigaSpawnPointRight.PauseSpawn();

        MiniBuldiga[] miniBuldigas = FindObjectsOfType<MiniBuldiga>();
        foreach (MiniBuldiga miniBuldiga in miniBuldigas)
        {
            StartCoroutine(DestroyMiniBuldigaWithDelay(miniBuldiga, Random.Range(0f, 1f)));
        }
    }

    private IEnumerator DestroyMiniBuldigaWithDelay(MiniBuldiga miniBuldiga, float delay)
    {
        yield return new WaitForSeconds(delay);
        miniBuldiga.TriggerDeath();
    }
}
