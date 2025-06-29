using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public abstract class RewardScoreAbstract : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI textObject;

    [SerializeField]
    private GameObject PlayAdSprite;

    [SerializeField]
    private GameObject LoadingCircle;

    protected RewardedAdManager rewardedAdManager;
    protected Button button;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Using FindFirstObjectByType because FindAnyObjectByType could return unexpected object in some cases, leading to 'rewardedAdManager.IsAdReady' being false
        rewardedAdManager = FindFirstObjectByType<RewardedAdManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        SetText();
        OnIsAdReadyChange(rewardedAdManager.IsAdReady);

        rewardedAdManager.OnIsAdReadyChange += OnIsAdReadyChange;
        rewardedAdManager.OnAdRewardGranted += OnAdRewardGrantedHandler;
    }

    protected virtual void OnDestroy()
    {
        rewardedAdManager.OnIsAdReadyChange -= OnIsAdReadyChange;
        rewardedAdManager.OnAdRewardGranted -= OnAdRewardGrantedHandler;
    }

    protected abstract void OnAdRewardGrantedHandler();

    protected virtual void OnIsAdReadyChange(bool newVal)
    {
        PlayAdSprite.SetActive(newVal);
        LoadingCircle.SetActive(!newVal);
    }

    protected void GrantReward(int scoreToAdd)
    {
        SettingsManager.instance.AddScore(scoreToAdd);
        SettingsManager.instance.AddExperience(scoreToAdd);

        AudioManager.instance.PlayCashSound();
    }

    protected virtual void OnClick()
    {
        rewardedAdManager.ShowRewardedAd();
    }

    protected abstract void SetText();

    protected void ButtonToggleInteractivity(bool status)
    {
        button.interactable = status;
    }
}
