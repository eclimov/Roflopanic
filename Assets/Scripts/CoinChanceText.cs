using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class CoinChanceText : MonoBehaviour
{
    [SerializeField] private LocalizedString localStringHighscore;
    [SerializeField] private TextMeshProUGUI textComp;

    private string chance;
    private SettingsManager settingsManager;

    private void OnEnable()
    {
        localStringHighscore.Arguments = new object[] { chance };
        localStringHighscore.StringChanged += UpdateText;
    }

    private void Start()
    {
        SetCoinChanceText();

        settingsManager = SettingsManager.instance;
        settingsManager.OnTotalScoreChange += SetCoinChanceText;
    }

    protected void OnDestroy()
    {
        settingsManager.OnTotalScoreChange -= SetCoinChanceText;
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }

    private void SetCoinChanceText()
    {
        localStringHighscore.Arguments[0] = (SettingsManager.GetCoinChance() * 100).ToString("F2");
        localStringHighscore.RefreshString();
    }
}
