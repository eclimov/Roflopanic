using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class MultiplyPoints : MonoBehaviour
{
    [SerializeField] private LocalizedString localString;
    [SerializeField] private TextMeshProUGUI textComp;

    private int value;

    private void OnEnable()
    {
        localString.Arguments = new object[] { value };
        localString.StringChanged += UpdateText;
    }

    private void UpdateText(string value)
    {
        textComp.text = value;
    }

    private void Start()
    {
        localString.Arguments[0] = SettingsManager.rewardPointsMultiplier;
        localString.RefreshString();
    }
}
