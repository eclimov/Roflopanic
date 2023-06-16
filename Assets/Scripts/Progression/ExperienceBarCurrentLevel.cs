using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ExperienceBarCurrentLevel : MonoBehaviour
{
    public TMP_Text playerLevelNumberText;
    public ExperienceBar experienceBar;

    public GameObject experienceText;

    public GameObject newLevelPanelPrefab;

    public TMP_Text levelNameText;
    public TMP_Text levelSloganText;
    public Image levelImage;

    private int playerLevelNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerLevelNumber = SettingsManager.GetPlayerLevelNumber();
        SetCurrentLevel();

        experienceBar.OnExperienceBarLevelChange += OnPlayerLevelChange;
    }

    protected void OnDestroy()
    {
        experienceBar.OnExperienceBarLevelChange -= OnPlayerLevelChange;
    }

    private void OnPlayerLevelChange(int sign)
    {
        playerLevelNumber += sign;

        if (Math.Sign(sign) > 0) // If the level is incrementing, spawn new level panel
        {
            AudioManager.instance.PlayFanfareSound();

            InitializeNewLevelPanel();
        }

        SetCurrentLevel();
    }

    public void InitializeNewLevelPanel() // This method should not have parameters, because it's being called from UI
    {
        Level level = GetCurrentLevel();

        GameObject newLevelPanelGameObject = Instantiate(newLevelPanelPrefab, GameObject.Find("Canvas").transform, false); // Spawn "New Level" panel

        NewLevelPanelHandler newLevelPanelHandler = newLevelPanelGameObject.GetComponent<NewLevelPanelHandler>();
        newLevelPanelHandler.InitializeLevel(level);
    }

    private Level GetCurrentLevel()
    {
        return SettingsManager.instance.GetPlayerLevelByNumber(playerLevelNumber);
    }


    private void SetCurrentLevel()
    {
        bool isLevelCap = playerLevelNumber == SettingsManager.instance.levels.Length;
        experienceText.SetActive(!isLevelCap);

        playerLevelNumberText.text = playerLevelNumber.ToString();

        Level currentLevel = GetCurrentLevel();
        levelNameText.text = currentLevel.GetName();
        levelSloganText.text = currentLevel.GetSlogan();
        levelImage.sprite = currentLevel.GetSprite();
    }
}
