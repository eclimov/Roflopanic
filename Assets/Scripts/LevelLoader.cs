using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public delegate void OnLevelLoadedDelegate();
    public event OnLevelLoadedDelegate OnLevelLoad;

    private AudioManager audioManager;
    private WaitForSeconds cachedWaitForSeconds;
    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();

        cachedWaitForSeconds = new WaitForSeconds(transitionTime);
        cachedWaitForSecondsRealtime = new WaitForSecondsRealtime(transitionTime);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        Time.timeScale = 1f; // Reset time scale (for cases when time-freeze panel appeared during loading)
        switch (scene.name)
        {
            case "Menu":
                audioManager.PlayMusicMenu();
                break;
            case "Gameplay":
                audioManager.PlayMusic("music-gameplay");
                break;
            case "Shop":
                audioManager.PlayMusic("music-shop");
                break;
        }

        StartCoroutine(OnSceneLoadedCoroutine());
    }

    private IEnumerator OnSceneLoadedCoroutine()
    {
        yield return cachedWaitForSeconds;

        if (OnLevelLoad != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnLevelLoad();
        }
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadLevel(0));
    }

    public void LoadGameplay()
    {
        StartCoroutine(LoadLevel(1));
    }

    public void LoadSettings()
    {
        StartCoroutine(LoadLevel(2));
    }
    public void LoadAbout()
    {
        StartCoroutine(LoadLevel(3));
    }

    public void LoadLevels()
    {
        StartCoroutine(LoadLevel(4));
    }

    public void LoadShop()
    {
        StartCoroutine(LoadLevel(5));
    }

    public void LoadVersus()
    {
        StartCoroutine(LoadLevel(6));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // Play Animation
        transition.SetTrigger("Start");

        // Wait
        // Waiting real seconds because of the possible timescale 0 (while the game is paused)
        // https://forum.unity.com/threads/waitforseconds-while-time-scale-0.272786/
        yield return cachedWaitForSecondsRealtime;

        // Load Scene Asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelIndex);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
