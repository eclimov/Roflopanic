using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using System;

public class TotalScore : MonoBehaviour
{
    [SerializeField] private LocalizedString localStringHighscore;
    [SerializeField] private TextMeshProUGUI textComp;

    private int totalScore = 0;

    private float increaseScore;
    private bool isSmoothIncrement;

    private void OnEnable()
    {
        localStringHighscore.Arguments = new object[] { totalScore };
        localStringHighscore.StringChanged += UpdateText;
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }

    private void Start()
    {
        totalScore = SettingsManager.totalScore;
        UpdateTotalScoreValue(totalScore);

        increaseScore = (float)totalScore;
    }

    private void UpdateTotalScoreValue(int value)
    {
        localStringHighscore.Arguments[0] = value;
        localStringHighscore.RefreshString();
    }

    private void Update()
    {
        if(isSmoothIncrement)
        {
            if (SettingsManager.totalScore > totalScore)
            {
                increaseScore = Mathf.MoveTowards(increaseScore, SettingsManager.totalScore, Time.deltaTime * SettingsManager.rewardScore);
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
