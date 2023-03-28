using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections.Generic;
using System.Collections;

public class ButtonRewardedAdHandler : MonoBehaviour
{
    private RewardedAdManager rewardedAdManager;
    private Button button;

    void Start()
    {
        rewardedAdManager = FindObjectOfType<RewardedAdManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        rewardedAdManager.ShowRewardedAd();
    }

    private void Update()
    {
        button.interactable = rewardedAdManager.isAdReady;
    }
}