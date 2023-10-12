using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public Level[] levels;
    public GameObject newLevelPanelPrefab;
    public GameObject confettiPrefab;

    public static ProgressionManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerLevelChange(Level level)
    {
        AudioManager.instance.PlayFanfareSound();
        InitializeNewLevelPanel(level);
        StartCoroutine(InitializeConfettiWithADelay(.5f));
    }

    public void InitializeNewLevelPanel(Level level)
    {
        GameObject newLevelPanelGameObject = Instantiate(newLevelPanelPrefab, GameObject.Find("Canvas UI").transform, false); // Spawn "New Level" panel

        NewLevelPanelHandler newLevelPanelHandler = newLevelPanelGameObject.GetComponent<NewLevelPanelHandler>();
        newLevelPanelHandler.InitializeLevel(level);
    }

    IEnumerator InitializeConfettiWithADelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Instantiate(confettiPrefab, GameObject.Find("Canvas UI").transform, false); // Spawn "Confetti" particles
    }
}
