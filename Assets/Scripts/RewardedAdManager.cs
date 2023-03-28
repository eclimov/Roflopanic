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
    public bool isAdReady;

    private RewardedAd rewardedAd;

    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;
    private string sceneName;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        sceneName = scene.name;
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

            isAdReady = rewardedAd != null
                && (sceneName == "Menu" || sceneName == "Gameplay" && GameOver.isEligibleForReward)
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
                isAdReady = false; // Disable button right after ad was watched, to prevent double watch

                SettingsManager settingsManager = FindObjectOfType<SettingsManager>();
                if (sceneName == "Menu")
                {
                    settingsManager.AddScore(SettingsManager.rewardScore);
                    FindObjectOfType<TotalScore>().SmoothValueIncrement();
                }
                else if (sceneName == "Gameplay")
                {
                    settingsManager.AddScore(
                        FindObjectOfType<ScoreManager>().GetScore() * (SettingsManager.rewardPointsMultiplier - 1) // Add the gathered score (multiplier - 1) times
                    );
                    GameOver.isEligibleForReward = false; // Do not allow multiplying more;
                }

                AudioManager.instance.PlayCashSound();
                FindObjectOfType<ProgressBar>().AnimateProgress(SettingsManager.totalScore, .1f);
            });
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }

    #endregion
}