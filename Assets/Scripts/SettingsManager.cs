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

    // data that should be stored in cloud
    public static SaveData.CloudSaveData SaveData;

    public static int targetTotalScore = 50_000;
    public static int maxCoinRateTotalScore = 1_000_000;

    public static ushort rewardScore = 300;
    public static byte rewardPointsMultiplier = 3;

    private bool isLocaleCoroutineActive = false;

    public static int difficultyId;
    private DifficultyMap[] difficultyMaps;

    public delegate void OnTotalScoreChangedDelegate();
    public event OnTotalScoreChangedDelegate OnTotalScoreChange;

    public delegate void OnHighscoreChangedDelegate();
    public event OnHighscoreChangedDelegate OnHighscoreChange;

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

            SaveData = new SaveData.CloudSaveData()
            {
                highscore = PlayerPrefs.GetInt("highscore", 0),
                totalScore = GetTotalScore()
            };

            /*
             * 0 - easy
             * 1 - medium
             * 2 - hard
            */
            difficultyId = PlayerPrefs.GetInt("difficultyId", 1); // Use Medium difficulty by default
            difficultyMaps = new DifficultyMap[]{
                new DifficultyMap(
                    backgroundSpeed: .25f,
                    obstacleTimeBetweenSpawn: .8f,
                    obstacleSpeed: 9f,
                    playerSpeed: 8f,
                    scoreIncrementMultiplier: 4f,
                    coinBonusScore: 20f
                ), // Easy
                new DifficultyMap(
                    backgroundSpeed: .6f,
                    obstacleTimeBetweenSpawn: .4f,
                    obstacleSpeed: 15f,
                    playerSpeed: 12f,
                    scoreIncrementMultiplier: 10f,
                    coinBonusScore: 50f
                ), // Medium

                new DifficultyMap(
                    backgroundSpeed: 1f,
                    obstacleTimeBetweenSpawn: .28f,
                    obstacleSpeed: 25f,
                    playerSpeed: 21f,
                    scoreIncrementMultiplier: 15f,
                    coinBonusScore: 70f
                ) // Hard
            };

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

    public static void SetDifficultyId(int value)
    {
        PlayerPrefs.SetInt("difficultyId", value);
        difficultyId = value;
    }

    public DifficultyMap GetDifficultyMap()
    {
        return difficultyMaps[difficultyId];
    }

    public static int GetTotalScore()
    {
        return PlayerPrefs.GetInt("totalScore", 0);
    }

    public static float GetCoinChance()
    {
        int totalScore = GetTotalScore();
        if(totalScore >= maxCoinRateTotalScore)
        {
            return 1f;
        }

        return (float)totalScore / (float)maxCoinRateTotalScore;
    }

    public bool IsTargetTotalScoreAchieved()
    {
        return GetTotalScore() >= targetTotalScore;
    }

    private static List<string> GetSeenGuideScenesList()
    {
        List<string> list = new List<string>();

        string isGuideSeenScenesPref = PlayerPrefs.GetString("isGuideSeenScenes", "");
        if(isGuideSeenScenesPref != "") // Otherwise, it will fill add an empty item to the list
        {
            list.AddRange(isGuideSeenScenesPref.Split(","));
        }

        return list;
    }

    public static bool IsSceneGuideSeen(string sceneName)
    {
        return GetSeenGuideScenesList().Contains(sceneName);
    }

    public static void SetSceneGuideSeen(string sceneName)
    {
        if(!IsSceneGuideSeen(sceneName))
        {
            List<string> list = GetSeenGuideScenesList();
            list.Add(sceneName);

            PlayerPrefs.SetString("isGuideSeenScenes", String.Join(",", list.ToArray()));
        }
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
        SaveData.highscore = newHighscore;
        PlayerPrefs.SetInt("highscore", newHighscore);

        if (OnHighscoreChange != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnHighscoreChange();
        }

        CloudSaveManager.Instance.Save();
    }

    private void SetTotalScore(int newTotalScore)
    {
        SaveData.totalScore = newTotalScore;
        PlayerPrefs.SetInt("totalScore", newTotalScore);

        if (OnTotalScoreChange != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnTotalScoreChange();
        }

        CloudSaveManager.Instance.Save();
    }

    public void AddScore(int scoreToAdd)
    {
        SetTotalScore(SaveData.totalScore + scoreToAdd);
    }

    public void DeleteData()
    {
        SetHighscore(0);
        SetTotalScore(0);
    }

    public void SaveSaveData(SaveData.CloudSaveData data)
    {
        SetHighscore(data.highscore);
        SetTotalScore(data.totalScore);
    }
}
