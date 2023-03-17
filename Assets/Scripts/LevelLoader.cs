using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadLevel(0, () => audioManager.PlayMusic("music-menu")));
    }

    public void LoadGameplay()
    {
        StartCoroutine(LoadLevel(1, () => audioManager.PlayMusic("music-gameplay")));
    }

    public void LoadSettings()
    {
        StartCoroutine(LoadLevel(2));
    }
    public void LoadAbout()
    {
        StartCoroutine(LoadLevel(3));
    }

    public void LoadThisLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator LoadLevel(int levelIndex, Action whenDone = null)
    {
        // Play Animation
        transition.SetTrigger("Start");

        // Wait
        // Waiting real seconds because of the possible timescale 0 (while the game is paused)
        // https://forum.unity.com/threads/waitforseconds-while-time-scale-0.272786/
        yield return new WaitForSecondsRealtime(transitionTime);

        // Load Scene Asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelIndex);

        whenDone?.Invoke();

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
