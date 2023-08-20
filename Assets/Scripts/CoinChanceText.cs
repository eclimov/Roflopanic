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
        SetCoinChanceText(SettingsManager.GetCoinChance());

        SettingsManager.instance.OnCoinChanceChange += SetCoinChanceText;
    }

    private void SetCoinChanceText(int coinChance)
    {
        localStringHighscore.Arguments[0] = coinChance;
        localStringHighscore.RefreshString();
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }
}
