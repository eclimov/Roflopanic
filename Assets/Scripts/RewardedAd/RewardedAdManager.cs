using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections.Generic;
using System.Collections;

public class RewardedAdManager : MonoBehaviour
{
    public static RewardedAdManager instance;
    
    private bool _isAdReady;
    public bool IsAdReady // Example: https://answers.unity.com/questions/1206632/trigger-event-on-variable-change.html
    {
        get { return _isAdReady; }
        set
        {
            if (_isAdReady == value) return;
            _isAdReady = value;
            if (OnIsAdReadyChange != null)
                OnIsAdReadyChange(_isAdReady);
        }
    }
    public delegate void OnIsAdReadyChangeDelegate(bool newVal);
    public event OnIsAdReadyChangeDelegate OnIsAdReadyChange;

    public delegate void OnAdRewardGrantedDelegate();
    public event OnAdRewardGrantedDelegate OnAdRewardGranted;

    private RewardedAd rewardedAd;

    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

            cachedWaitForSecondsRealtime = new WaitForSecondsRealtime(3f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(InfiniteRefreshRewardedAd());
    }

    private IEnumerator InfiniteRefreshRewardedAd()
    {
        for (; ; )
        {
            if (SettingsManager.isMobileAdsSDKInitialized && (rewardedAd == null || !rewardedAd.CanShowAd()))
            {
                RequestAndLoadRewardedAd();
            }

            IsAdReady = rewardedAd != null
                && !(Application.internetReachability == NetworkReachability.NotReachable)
                && rewardedAd.CanShowAd();

            yield return cachedWaitForSecondsRealtime;
        }
    }

    private void OnDestroy()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("game")
            .AddKeyword("humor")
            .Build();
    }

    #endregion

    #region REWARDED ADS

    private void RequestAndLoadRewardedAd()
    {
        Debug.Log("Requesting Rewarded ad.");
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-8932201865618296/2405946490";
#else
        string adUnitId = "unused";
#endif

        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Rewarded ad failed to load with error: " +
                                loadError.GetMessage());

                    rewardedAd = null; // Reset the instance to reload the ad
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Rewarded ad failed to load.");

                    rewardedAd = null; // Reset the instance to reload the ad
                    return;
                }

                // Rewarded ad loaded.
                rewardedAd = ad;

                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log("Rewarded ad failed to show with error: " +
                               error.GetMessage());

                    rewardedAd = null; // Reset the instance to reload the ad
                };
            });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            // Rewarded ad granted a reward
            rewardedAd.Show((Reward reward) =>
            {
                IsAdReady = false; // Disable button right after ad was watched, to prevent double watch

                if (OnAdRewardGranted != null) // It is a MUST to check this, because the event is null if it has no subscribers
                {
                    OnAdRewardGranted();
                }
            });
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }

    #endregion
}