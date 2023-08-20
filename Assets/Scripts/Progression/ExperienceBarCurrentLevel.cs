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

    public GameObject newLevelPanelPrefab; // TODO: remove?
    public GameObject confettiPrefab; // TODO: remove?

    public TMP_Text levelNameText;
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
            ProgressionManager.instance.OnPlayerLevelChange(GetCurrentLevel());
        }

        SetCurrentLevel();
    }

    public void InitializeNewLevelPanel() // This method should not have parameters, because it's being called from UI
    {
        ProgressionManager.instance.InitializeNewLevelPanel(GetCurrentLevel());
    }

    private Level GetCurrentLevel()
    {
        return SettingsManager.instance.GetPlayerLevelByNumber(playerLevelNumber);
    }


    private void SetCurrentLevel()
    {
        bool isLevelCap = playerLevelNumber == ProgressionManager.instance.levels.Length;
        experienceText.SetActive(!isLevelCap);

        playerLevelNumberText.text = playerLevelNumber.ToString();

        Level currentLevel = GetCurrentLevel();
        levelNameText.text = currentLevel.GetName();
        levelImage.sprite = currentLevel.GetSprite();
    }
}
