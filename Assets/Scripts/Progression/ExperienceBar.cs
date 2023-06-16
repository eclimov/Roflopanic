using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ExperienceBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text experienceText;
    public Animator GlintAnimator;

    private int currentExperience;

    private int remainingExperienceToFill = 0;

    private SettingsManager settingsManager;

    public delegate void OnExperienceBarLevelChangeDelegate(int sign);
    public event OnExperienceBarLevelChangeDelegate OnExperienceBarLevelChange;

    private void Start()
    {
        settingsManager = SettingsManager.instance;
        
        currentExperience = SettingsManager.SaveData.experience;
        SetExperienceBarValue();

        settingsManager.OnExperienceChange += OnExperienceChangeHandler;
    }

    protected void OnDestroy()
    {
        settingsManager.OnExperienceChange -= OnExperienceChangeHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if(remainingExperienceToFill != 0 
            && Time.timeScale > 0 // Do not increase experience bar when game is paused (or a panel is displayed)
        )
        {
            int remainingExperienceToFillSign = Math.Sign(remainingExperienceToFill);

            currentExperience += remainingExperienceToFillSign;

            float sliderValueBeforeUpdate = slider.value;
            SetExperienceBarValue();
            if(sliderValueBeforeUpdate > slider.value && remainingExperienceToFillSign > 0
                || sliderValueBeforeUpdate < slider.value && remainingExperienceToFillSign < 0
            ) // Level has changed
            {
                if (OnExperienceBarLevelChange != null) // It is a MUST to check this, because the event is null if it has no subscribers
                {
                    OnExperienceBarLevelChange(remainingExperienceToFillSign);
                }
            }

            remainingExperienceToFill -= remainingExperienceToFillSign;
        } else
        {
            if (GlintAnimator.GetBool("IsMoving"))
            {
                GlintAnimator.SetBool("IsMoving", false);
            }
        }
    }

    private void OnExperienceChangeHandler(int newExperience)
    {
        StartCoroutine(AnimateExperienceBarWithADelay(newExperience, 1f));
    }

    IEnumerator AnimateExperienceBarWithADelay(int targetExperience, float delay)
    {
        yield return new WaitForSeconds(delay);
        AnimateExperience(targetExperience);
    }

    public void AnimateExperience(int targetExperience)
    {
        GlintAnimator.SetBool("IsMoving", true);
        remainingExperienceToFill = targetExperience - currentExperience;
    }

    private void SetExperienceBarValue()
    {
        int currentExperienceBarValue = currentExperience % SettingsManager.experiencePerLevel;
        slider.value = Mathf.Floor(
            ((float)currentExperienceBarValue / SettingsManager.experiencePerLevel) * 100f
        ) / 100f;

        experienceText.text = currentExperienceBarValue.ToString() + "/" + SettingsManager.experiencePerLevel.ToString();
    }
}
