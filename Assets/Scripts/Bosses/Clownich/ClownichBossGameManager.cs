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

    protected override void Start()
    {
        base.Start();

        MakeBossInvulnerable(); // Call the method to make sure the shield is enabled, and display info text in the healthbar accordingly
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

        yield return base.BossFightWon();
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
