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

    public delegate void OnGameOverDelegate();
    public event OnGameOverDelegate OnGameOver;

    private GameObject playerGameObject;
    private bool canReincarnate;

    private void Awake()
    {
        isGameOver = false; // Leave it here, to reset the value on restart
        playerGameObject = GameObject.FindGameObjectWithTag("Player").gameObject;
        canReincarnate = SettingsManager.IsAbilityEquipped("reincarnation");
    }

    private IEnumerator Reincarnate()
    {
        BackgroundGameplay background = FindObjectOfType<BackgroundGameplay>();
        background.NegateScrolling();

        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        spawner.isReadyToEmit = false; // Stop spawning new items

        AbstractSpawnable[] spawnables = FindObjectsOfType<AbstractSpawnable>()
            .OrderBy((item) => item.transform.position.x).ToArray(); // Order by X position, so the leftmost element would be first

        foreach (AbstractSpawnable obj in spawnables) // Move all spawnables in the opposite directon
        {
            obj.NegateMovement();
        }

        AudioManager.instance.PlayMusic("music-gameplay-reincarnation");

        timeFreezeOverlayPanel.SetActive(true);

        yield return new WaitUntil(() => spawnables[0].transform.position.x >= 15); // Position X is relative here: -10 - left border, 0 - center, 10 - right border

        timeFreezeOverlayPanel.GetComponent<TimeFreezeOverlayPanel>().FadeOut();

        yield return new WaitForSecondsRealtime(1f);

        AudioManager.instance.PlayButtonLipsSound();

        playerGameObject.SetActive(true);
        playerGameObject.GetComponent<Player>().OnReincarnate();
        background.NegateScrolling();

        foreach (AbstractSpawnable obj in spawnables) // Move all spawnables in the correct directon
        {
            obj.NegateMovement();
            obj.gameObject.SetActive(false); // Disable all spawnables to avoid overlaying 
        }

        AudioManager.instance.PlayMusic("music-gameplay");

        yield return new WaitForSecondsRealtime(1f);

        canReincarnate = false; // Cannot reincarnate anymore during current game
        spawner.isReadyToEmit = true;
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
