using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    public Image levelImage;

    public GameObject newLevelPanelPrefab;

    private Level myLevel;

    public void InitializeLevel(Level level)
    {
        myLevel = level;

        if(SettingsManager.GetPlayerLevelNumber() < myLevel.GetNumber())
        {
            Button button = GetComponent<Button>();
            button.interactable = false;
        } else
        {
            levelImage.sprite = level.GetSprite();
        }
    }

    public void SpawnLevelPanel() // This method should not have parameters, because it's being called from UI
    {
        GameObject newLevelPanelGameObject = Instantiate(newLevelPanelPrefab, GameObject.Find("Canvas").transform, false); // Spawn "New Level" panel

        NewLevelPanelHandler newLevelPanelHandler = newLevelPanelGameObject.GetComponent<NewLevelPanelHandler>();
        newLevelPanelHandler.InitializeLevel(myLevel);
    }
}
