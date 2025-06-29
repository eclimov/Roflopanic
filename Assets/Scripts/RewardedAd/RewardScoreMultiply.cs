using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardScoreMultiply : RewardScoreAbstract
{
    private bool wasClicked; // The button should trigger only once

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnClick()
    {
        base.OnClick();

        wasClicked = true;
    }

    protected override void SetText()
    {
        textObject.text = "×" + SettingsManager.rewardPointsMultiplier;
    }

    protected override void OnAdRewardGrantedHandler()
    {
        int scoreToAdd = FindAnyObjectByType<ScoreManager>().GetScore() * (SettingsManager.rewardPointsMultiplier - 1); // Add the gathered score (multiplier - 1) times
        GrantReward(scoreToAdd);
    }

    protected override void OnIsAdReadyChange(bool newVal)
    {
        base.OnIsAdReadyChange(newVal);

        ButtonToggleInteractivity(newVal && !wasClicked);
    }
}
