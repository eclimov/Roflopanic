using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using GoogleMobileAds.Api;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public static bool isMobileAdsSDKInitialized;
    public static bool isMusicEnabled;
    public static bool isSoundEnabled;
    public static bool isVibrationEnabled;
    public static ushort localeId;
    public static int highscore;
    public static int totalScore;

    public static int targetTotalScore = 300;

    public static ushort rewardScore = 300;
    public static byte rewardPointsMultiplier = 3;

    private bool isLocaleCoroutineActive = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

            isMusicEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("isMusicEnabled", 1));
            isSoundEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("isSoundEnabled", 1));
            isVibrationEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("isVibrationEnabled", 1));
            localeId = LocalizationSettings.SelectedLocale.SortOrder;
            highscore = PlayerPrefs.GetInt("highscore", 0);
            totalScore = GetTotalScore();

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
                isMobileAdsSDKInitialized = true;
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "Settings") // If current scene was "settings", save the settings before exit, just in case: https://gamedevbeginner.com/how-to-use-player-prefs-in-unity/#save_playerprefs
        {
            PlayerPrefs.Save();
        }
    }

    public static int GetTotalScore()
    {
        return PlayerPrefs.GetInt("totalScore", 0);
    }

    public bool IsTargetTotalScoreAchieved()
    {
        return GetTotalScore() >= targetTotalScore;
    }

    public void setMusicEnabled(bool isEnabled)
    {
        isMusicEnabled = isEnabled;
        PlayerPrefs.SetInt("isMusicEnabled", Convert.ToInt32(isMusicEnabled));

        FindObjectOfType<AudioManager>().LoadMusicSettings();
    }

    public void setSoundEnabled(bool isEnabled)
    {
        isSoundEnabled = isEnabled;
        PlayerPrefs.SetInt("isSoundEnabled", Convert.ToInt32(isSoundEnabled));

        FindObjectOfType<AudioManager>().PlayButtonSound();
    }

    public void setVibrationEnabled(bool isEnabled)
    {
        isVibrationEnabled = isEnabled;
        PlayerPrefs.SetInt("isVibrationEnabled", Convert.ToInt32(isVibrationEnabled));
        if(isVibrationEnabled)
        {
            Vibration.Vibrate(100);
        }
    }

    public void ChangeLocale(ushort localeID)
    {
        if (isLocaleCoroutineActive == true) // Avoid calling the coroutine multiple times
        {
            return;
        }
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(ushort _localeID)
    {
        isLocaleCoroutineActive = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        localeId = _localeID;
        isLocaleCoroutineActive = false;
    }

    public void SetHighscore(int newHighscore)
    {
        highscore = newHighscore;
        PlayerPrefs.SetInt("highscore", highscore);
    }

    public void AddScore(int scoreToAdd)
    {
        totalScore += scoreToAdd;
        PlayerPrefs.SetInt("totalScore", totalScore);
    }
}
