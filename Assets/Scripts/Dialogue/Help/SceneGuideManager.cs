using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneGuideManager : MonoBehaviour
{
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

        GameObject sceneGuideButtonGameObject = GameObject.Find("Scene Guide Button");
        if (sceneGuideButtonGameObject != null && sceneGuideButtonGameObject.TryGetComponent<InfoButtonHandler>(out InfoButtonHandler sceneGuideButton))
        {
            GameObject helpPanel = sceneGuideButton.SpawnDialogue();
            helpPanel.GetComponent<HelpPanelHandler>().OnPanelClose += (() => {
                SettingsManager.SetSceneGuideSeen(currentSceneName);
            });
        }
    }
}
