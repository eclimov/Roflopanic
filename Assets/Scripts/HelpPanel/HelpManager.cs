using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpManager : MonoBehaviour
{
    public GameObject helpPanelPrefab;
    public Dialogue dialogue;

    private LevelLoader levelLoader;
    private string currentSceneName;

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.OnLevelLoad += OnLevelLoaded;
    }

    protected void OnDestroy()
    {
        levelLoader.OnLevelLoad -= OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        if (SettingsManager.IsSceneGuideSeen(currentSceneName))
        {
            return;
        }

        SpawnHelpPanel();
    }

    public void SpawnHelpPanel()
    {
        GameObject obj = Instantiate(helpPanelPrefab, GameObject.Find("Canvas").transform, false);
        obj.GetComponent<HelpPanelHandler>().OnPanelClose += (() => {
            SettingsManager.SetSceneGuideSeen(currentSceneName);
        });

        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
