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
    [SerializeField]
    private GameObject PlayAdSprite;

    [SerializeField]
    private GameObject LoadingCircle;

    [SerializeField]
    private bool isOneTimeTrigger;
    private bool wasClicked;

    private RewardedAdManager rewardedAdManager;
    private Button button;

    void Start()
    {
        rewardedAdManager = FindObjectOfType<RewardedAdManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        SetInteractible(rewardedAdManager.IsAdReady);
        rewardedAdManager.OnIsAdReadyChange += SetInteractible;
    }

    protected void OnDestroy()
    {
        rewardedAdManager.OnIsAdReadyChange -= SetInteractible; // Unsubscribe to avoid nullpointerexception
    }

    private void OnClick()
    {
        rewardedAdManager.ShowRewardedAd();
        wasClicked = true;
    }

    private void SetInteractible(bool status)
    {
        if(isOneTimeTrigger && wasClicked) // If the button is a one-time trigger and it was clicked, do not allow clickiung it anymore, but show the static sprite
        {
            PlayAdSprite.SetActive(true);
            LoadingCircle.SetActive(false);
            button.interactable = false;
        } else
        {
            PlayAdSprite.SetActive(status);
            LoadingCircle.SetActive(!status);
            button.interactable = status;
        }
    }
}