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

    private void OnEnable()
    {
        localStringHighscore.Arguments = new object[] { chance };
        localStringHighscore.StringChanged += UpdateText;
    }

    private void Start()
    {
        SetCoinChanceText();
    }

    private void SetCoinChanceText()
    {
        localStringHighscore.Arguments[0] = SettingsManager.GetCoinChance();
        localStringHighscore.RefreshString();
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }
}
