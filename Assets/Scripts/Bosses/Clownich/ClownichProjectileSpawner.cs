using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownichProjectileSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;

    private bool canSpawn = true;

    public void AllowSpawn(bool isAllowed)
    {
        canSpawn = isAllowed;
    }

    public void SpawnProjectile()
    {
        if (!canSpawn) return;

        AudioManager.instance.PlaySwooshSound();

        GameObject projectileGameObject = Instantiate(projectilePrefab, FindObjectOfType<ClownichBossGameManager>().gameObject.transform);
        projectileGameObject.transform.position = gameObject.transform.position;

        projectileGameObject.GetComponent<ClownichProjectile>().SetColor(PickNextProjectileColor());
    }

    private Color32 PickNextProjectileColor()
    {
        float randomNumber = Random.Range(1, 7);
        switch (randomNumber)
        {
            case 1:
                return new Color32(159, 46, 153, 255); // Violet

            case 2:
                return new Color32(45, 102, 255, 255); // Blue

            case 3:
                return new Color32(1, 151, 27, 255); // Green

            case 4:
                return new Color32(247, 247, 62, 255); // Yellow

            case 5:
                return new Color32(254, 122, 25, 255); // Orange

            default:
                return new Color32(255, 1, 7, 255); // Red
        }
    }
}
