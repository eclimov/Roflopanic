using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGuideManager : MonoBehaviour
{
    private LevelLoader levelLoader;
    private string currentSceneName;

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        levelLoader = FindAnyObjectByType<LevelLoader>();
        levelLoader.OnLevelLoad += ShowSceneGuide;
    }

    protected void OnDestroy()
    {
        levelLoader.OnLevelLoad -= ShowSceneGuide;
    }

    private void ShowSceneGuide()
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
                SettingsManager.instance.SetSceneGuideSeen(currentSceneName);
            });
        }
    }
}
