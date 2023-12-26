using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusProjectileSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;

    public void SpawnProjectile(Vector2 direction, string emittingPlayerName)
    {
        AudioManager.instance.PlaySwooshSound();

        GameObject projectileGameObject = Instantiate(projectilePrefab, FindObjectOfType<VersusPhase2GameManager>().gameObject.transform);
        projectileGameObject.transform.position = gameObject.transform.position;

        VersusProjectile versusProjectile = projectileGameObject.GetComponent<VersusProjectile>();
        versusProjectile.ThrowAt(direction);
        versusProjectile.SetEmittingPlayerName(emittingPlayerName);
    }
}
