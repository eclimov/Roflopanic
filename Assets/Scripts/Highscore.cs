using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using System;

public class Highscore : MonoBehaviour
{
    [SerializeField] private LocalizedString localStringHighscore;
    [SerializeField] private TextMeshProUGUI textComp;

    private int highscore;

    private void OnEnable()
    {
        localStringHighscore.Arguments = new object[] { highscore };
        localStringHighscore.StringChanged += UpdateText;
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }

    private void Start()
    {
        SetHighscoreText();

        SettingsManager.instance.OnHighscoreChange += SetHighscoreText;
    }

    protected void OnDestroy()
    {
        SettingsManager.instance.OnHighscoreChange -= SetHighscoreText;
    }

    private void SetHighscoreText()
    {
        localStringHighscore.Arguments[0] = SettingsManager.SaveData.highscore;
        localStringHighscore.RefreshString();
    }
}
