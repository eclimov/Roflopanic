using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class ButtonRewardedAdHandler : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI textObject;

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

        switch(SceneManager.GetActiveScene().name)
        {
            case "Menu":
                textObject.text = "+" + SettingsManager.rewardScore;
                break;
            default: // Gameplay
                textObject.text = "×" + SettingsManager.rewardPointsMultiplier;
                break;
        }

        SetInteractible(rewardedAdManager.IsAdReady);
        rewardedAdManager.OnIsAdReadyChange += SetInteractible;
        rewardedAdManager.OnAdRewardGranted += OnAdRewardGrantedHandler;
    }

    protected void OnDestroy()
    {
        rewardedAdManager.OnIsAdReadyChange -= SetInteractible; // Unsubscribe to avoid nullpointerexception
        rewardedAdManager.OnAdRewardGranted -= OnAdRewardGrantedHandler;
    }

    private void OnAdRewardGrantedHandler()
    {
        int scoreToAdd;
        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
                scoreToAdd = SettingsManager.rewardScore;
                break;
            case "Gameplay":
                scoreToAdd = FindObjectOfType<ScoreManager>().GetScore() * (SettingsManager.rewardPointsMultiplier - 1); // Add the gathered score (multiplier - 1) times
                break;
            default:
                scoreToAdd = 0;
                break;
        }

        SettingsManager.instance.AddScore(scoreToAdd);
        SettingsManager.instance.AddExperience(scoreToAdd);

        AudioManager.instance.PlayCashSound();
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