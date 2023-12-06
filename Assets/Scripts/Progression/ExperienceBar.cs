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
    public AudioSource audioSource;
    public GameObject santaHat;

    private float currentExperience;
    private float targetExperience;
    private float maxDeltaMultiplier;

    private int experienceMovementSign = 1;

    private SettingsManager settingsManager;

    public delegate void OnExperienceBarLevelChangeDelegate(int sign);
    public event OnExperienceBarLevelChangeDelegate OnExperienceBarLevelChange;

    private void Start()
    {
        settingsManager = SettingsManager.instance;
        
        currentExperience = SettingsManager.SaveData.experience;
        targetExperience = currentExperience;
        maxDeltaMultiplier = 1;

        santaHat.SetActive(SettingsManager.IsChristmasTime());

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
        if(
            Time.timeScale > 0 // Do not do anything when game is paused (or a panel is displayed)
        )
        {
            if (currentExperience != targetExperience)
            {
                currentExperience = Mathf.MoveTowards(currentExperience, targetExperience, Time.deltaTime * maxDeltaMultiplier);

                float sliderValueBeforeUpdate = slider.value;
                SetExperienceBarValue();
                if (sliderValueBeforeUpdate > slider.value && experienceMovementSign > 0
                    || sliderValueBeforeUpdate < slider.value && experienceMovementSign < 0
                ) // Level has changed
                {
                    if (OnExperienceBarLevelChange != null) // It is a MUST to check this, because the event is null if it has no subscribers
                    {
                        OnExperienceBarLevelChange(experienceMovementSign);
                    }
                }
            }
            else
            {
                if (GlintAnimator.GetBool("IsMoving"))
                {
                    GlintAnimator.SetBool("IsMoving", false);
                }
            }
        }
    }

    private void OnExperienceChangeHandler(int newExperience)
    {
        StartCoroutine(AnimateExperienceBarWithADelay(newExperience, 1f));
    }

    IEnumerator AnimateExperienceBarWithADelay(int newExperience, float delay)
    {
        yield return new WaitForSeconds(delay);

        GlintAnimator.SetBool("IsMoving", true);
        targetExperience = newExperience;

        float differenceExperience = Math.Abs(targetExperience - currentExperience);
        maxDeltaMultiplier = differenceExperience / 2;
        // The transition speed should not be too slow or too fast
        if (maxDeltaMultiplier < 10)
        {
            maxDeltaMultiplier = 10; // Minimum multiplier
        } else if(maxDeltaMultiplier > 10000)
        {
            maxDeltaMultiplier = 10000; // Maximum multiplier
        }

        experienceMovementSign = Math.Sign(targetExperience - currentExperience);
        StartCoroutine(PlayExperienceSound(experienceMovementSign));
    }

    protected IEnumerator PlayExperienceSound(int sign)
    {
        float pitch = 1f;
        float minPitch = .5f;
        float maxPitch = 1.5f;
        WaitForSecondsRealtime cachedIntervalBetweenSoundTicks = new WaitForSecondsRealtime(.05f);

        while (currentExperience != targetExperience)
        {
            if (Time.timeScale > 0) // Do not do anything when game is paused (or a panel is displayed)
            {
                if(SettingsManager.isSoundEnabled)
                {
                    audioSource.pitch = pitch;
                    audioSource.PlayOneShot(audioSource.clip);
                    if (pitch >= minPitch && pitch <= maxPitch)
                    {
                        pitch += .01f * sign;
                    }
                }
            }

            yield return cachedIntervalBetweenSoundTicks;
        }

        yield return new WaitForSecondsRealtime(.5f);
        audioSource.pitch = 1f;
    }

    private void SetExperienceBarValue()
    {
        int currentExperienceBarValue = (int)currentExperience % SettingsManager.experiencePerLevel;
        slider.value = Mathf.Floor(
            ((float)currentExperienceBarValue / SettingsManager.experiencePerLevel) * 100f
        ) / 100f;

        experienceText.text = currentExperienceBarValue.ToString() + "/" + SettingsManager.experiencePerLevel.ToString();
    }
}
