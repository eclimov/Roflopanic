using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public GameObject CrownSpriteObject;
    public GameObject TargetTotalScoreText;
    public enum DefaultValueEnum { 
        Zero, 
        CurrentScore
    };
    public DefaultValueEnum DefaultValue = DefaultValueEnum.Zero;

    public GameObject ProgressBarGlint;

    private Slider slider;

    private float targetTotalScore;

    private float FillSpeed = .1f;
    private float targetProgress = 0f;

    private Animator crownAnimator;
    private Animator progressBarGlintAnimator;

    private void Awake()
    {
        targetTotalScore = (float)SettingsManager.targetTotalScore;

        slider = gameObject.GetComponent<Slider>();
        switch (DefaultValue)
        {
            case DefaultValueEnum.CurrentScore:
                slider.value = CalculateProgressByValue(SettingsManager.totalScore);
                break;
            default:
                slider.value = 0f;
                break;
        }

        crownAnimator = CrownSpriteObject.GetComponent<Animator>();
        progressBarGlintAnimator = ProgressBarGlint.GetComponent<Animator>();

        TargetTotalScoreText.GetComponent<TMP_Text>().text = targetTotalScore.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        if (slider.value < targetProgress)
        {
            slider.value += FillSpeed * Time.deltaTime;
        } else
        {
            if (progressBarGlintAnimator.GetBool("IsMoving"))
            {
                progressBarGlintAnimator.SetBool("IsMoving", false);
            }
        }

        if (slider.value >= 1f && !crownAnimator.GetCurrentAnimatorStateInfo(0).IsName("Crown_Enable")) // Prevent entering the condition multiple times
        {
            CrownSpriteObject.GetComponent<Animator>().SetTrigger("Enable");
        }
    }

    public void AnimateProgress(float targetValue, float customFillSpeed)
    {
        progressBarGlintAnimator.SetBool("IsMoving", true);

        FillSpeed = customFillSpeed;

        float newProgress = 1f; // Default value
        if(targetValue < targetTotalScore)
        {
            newProgress = CalculateProgressByValue(targetValue);
        }
        targetProgress = newProgress;
    }

    private float CalculateProgressByValue(float value)
    {
        return Mathf.Floor(
            (value / targetTotalScore) * 100f
        ) / 100f;
    }
}
