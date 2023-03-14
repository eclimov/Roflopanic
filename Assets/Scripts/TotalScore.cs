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

    private int totalScore;

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
        localStringHighscore.Arguments[0] = SettingsManager.totalScore;
        localStringHighscore.RefreshString();
    }
}
