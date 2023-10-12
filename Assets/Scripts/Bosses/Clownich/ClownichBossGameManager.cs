using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClownichBossGameManager : AbstractBossGameManager
{
    public List<ClownichGate> allyClownichGates; // Make sure the order is natural (1, 2, 3, etc.)
    public ClownichGate enemyClownichGate;

    public static string bossFightsWonCountKey = "bossFightsWonClownich";

    private ClownichProjectileSpawner clownichProjectileSpawner;

    protected override void Awake()
    {
        base.Awake();

        clownichProjectileSpawner = FindObjectOfType<ClownichProjectileSpawner>();
    }

    protected override void OnGameOverHandler()
    {
        clownichProjectileSpawner.AllowSpawn(false);
        DestroyAllProjectiles();
    }

    protected override void OnReincarnationStartedHandler()
    {
        clownichProjectileSpawner.AllowSpawn(false);
        DestroyAllProjectiles();
    }

    protected override void OnReincarnationEndedHandler()
    {
        clownichProjectileSpawner.AllowSpawn(true);
        EnableTheLastAllyGate();
    }

    protected override IEnumerator BossFightWon()
    {
        DestroyAllProjectiles();
        DestroyAllGates();

        boss.Die();

        yield return new WaitForSecondsRealtime(1f); // A delay between death animation and victory text

        AudioManager.instance.PauseMusic();

        BossVictoryText bossVictoryText = Instantiate(bossVictoryTextPrefab, GameObject.Find("Canvas").transform).GetComponent<BossVictoryText>();
        bossVictoryText.SetBossName(bossName);
        bossVictoryText.FadeIn();

        yield return new WaitForSecondsRealtime(3f); // Wait for fadeIn to finish and a little bit more

        Destroy(bossHealthManager.gameObject); // Destroy boss health bar (because it's not a child of the current gameobject)

        bossVictoryText.FadeOut();
        yield return new WaitForSecondsRealtime(1f); // Wait for fadeOut to finish

        AudioManager.instance.PlayCashSound();
        scoreManager.AddBonusScore(bossRewardScore);

        gameManager.EndBossFight(gameObject); // This should go the last, because boss game manager is destroyed there
    }

    protected override void IncrementBossFightWinCount()
    {
        SettingsManager.IncrementBossFightsWonCount(bossFightsWonCountKey);
    }

    private void DestroyAllProjectiles()
    {
        ClownichProjectile[] clownichProjectiles = FindObjectsOfType<ClownichProjectile>();
        foreach (ClownichProjectile clownichProjectile in clownichProjectiles)
        {
            clownichProjectile.DestroyWithExplosion();
        }
    }

    private void DestroyAllGates()
    {
        ClownichGate[] clownichGates = FindObjectsOfType<ClownichGate>();
        foreach (ClownichGate clownichGate in clownichGates)
        {
            Destroy(clownichGate.gameObject);
        }
    }

    // Use this method to destroy, to make sure "projectile to gate" collision doesn't affect multiple gates nearby
    public void DisableNextAllyGate()
    {
        AudioManager.instance.PlayDeathSound();
        allyClownichGates.Find(e => e.gameObject.activeSelf).gameObject.SetActive(false);
        
        if(allyClownichGates.TrueForAll(e => !e.gameObject.activeSelf)) // If no ally gates remain
        {
            BossFightLost();
        }
    }

    private void EnableTheLastAllyGate()
    {
        allyClownichGates.Last().gameObject.SetActive(true);
    }

    public override void MakeBossVulnerable()
    {
        base.MakeBossVulnerable();

        enemyClownichGate.gameObject.SetActive(false);
    }
}
