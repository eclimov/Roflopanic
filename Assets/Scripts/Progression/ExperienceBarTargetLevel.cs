using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ExperienceBarTargetLevel : MonoBehaviour
{
    public TMP_Text targetLevelText;
    public ExperienceBar experienceBar;

    private int targetPlayerLevel;

    // Start is called before the first frame update
    void Start()
    {
        targetPlayerLevel = SettingsManager.GetPlayerLevelNumber() + 1;
        SetTargetLevel();

        experienceBar.OnExperienceBarLevelChange += OnPlayerLevelChange;
    }

    protected void OnDestroy()
    {
        experienceBar.OnExperienceBarLevelChange -= OnPlayerLevelChange;
    }

    private void OnPlayerLevelChange(int sign)
    {
        targetPlayerLevel += sign;

        SetTargetLevel();
    }

    private void SetTargetLevel()
    {
        gameObject.SetActive(targetPlayerLevel <= SettingsManager.instance.levels.Length);
        targetLevelText.text = targetPlayerLevel.ToString();
    }
}
