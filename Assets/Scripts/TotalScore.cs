using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using System;

public class TotalScore : MonoBehaviour
{
    [SerializeField] private LocalizedString localStringTotalScore;
    [SerializeField] private TextMeshProUGUI textComp;

    private int totalScore = 0;

    private float increaseScore;
    private bool isSmoothIncrement;

    private void OnEnable()
    {
        localStringTotalScore.Arguments = new object[] { totalScore };
        localStringTotalScore.StringChanged += UpdateText;
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }

    private void Start()
    {
        totalScore = SettingsManager.SaveData.totalScore;
        UpdateTotalScoreValue(totalScore);

        increaseScore = (float)totalScore;

        SettingsManager.instance.OnTotalScoreChange += SmoothValueIncrement;
    }

    protected void OnDestroy()
    {
        SettingsManager.instance.OnTotalScoreChange -= SmoothValueIncrement;
    }

    private void UpdateTotalScoreValue(int value)
    {
        localStringTotalScore.Arguments[0] = value;
        localStringTotalScore.RefreshString();
    }

    private void Update()
    {
        if(isSmoothIncrement)
        {
            if (SettingsManager.SaveData.totalScore != totalScore)
            {
                increaseScore = Mathf.MoveTowards(increaseScore, SettingsManager.SaveData.totalScore, Time.deltaTime * SettingsManager.rewardScore);
                totalScore = (int)increaseScore;
                UpdateTotalScoreValue(totalScore);
            } else
            {
                isSmoothIncrement = false;
            }
        }
    }

    public void SmoothValueIncrement()
    {
        isSmoothIncrement = true;
    }
}
