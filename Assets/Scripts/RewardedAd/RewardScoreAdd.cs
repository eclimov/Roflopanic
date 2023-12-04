using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RewardScoreAdd : RewardScoreAbstract
{
    [SerializeField]
    private int timerCooldonwnTime; // Seconds

    [SerializeField]
    private TimerText timerText;

    protected override void Start()
    {
        InitializeTimer(); // Keep this instruction first, to make sure timer is enabled before base Start is called

        base.Start();

        timerText.OnTimeout += OnTimerTimeout;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        timerText.OnTimeout -= OnTimerTimeout;
    }

    private void InitializeTimer()
    {
        if (timerCooldonwnTime > 0)
        {
            int rewardedAdSeenTimeDifference = SettingsManager.GetRewardedAdSeenTimeDifference();
            if (rewardedAdSeenTimeDifference < timerCooldonwnTime)
            {
                ToggleTimer(true); // WARNING: avoid enabling TextMeshPro instantly after google AD, because separate thread makes the app crash (see InitializeTimerDelayed)
                timerText.SetTimer(timerCooldonwnTime - rewardedAdSeenTimeDifference);
            }
        }
    }

    // Purpose: to prevent crashing the application https://forum.unity.com/threads/graphics-device-is-null-tmpro-textmeshprougui-awake.1371333/
    private IEnumerator InitializeTimerDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitializeTimer();
    }

    private void OnTimerTimeout()
    {
        ToggleTimer(false);
        OnIsAdReadyChange(rewardedAdManager.IsAdReady);
    }

    protected override void SetText()
    {
        textObject.text = "+" + SettingsManager.rewardScore;
    }

    private void ToggleTimer(bool status)
    {
        timerText.gameObject.SetActive(status);
        textObject.gameObject.SetActive(!status);
    }

    protected override void OnAdRewardGrantedHandler()
    {
        int scoreToAdd = SettingsManager.rewardScore;
        GrantReward(scoreToAdd);

        SettingsManager.SetRewardedAdSeenTime();
        StartCoroutine(InitializeTimerDelayed(.1f));
    }

    protected override void OnIsAdReadyChange(bool newVal)
    {
        base.OnIsAdReadyChange(newVal);

        ButtonToggleInteractivity(newVal && !timerText.gameObject.activeSelf);
    }
}
