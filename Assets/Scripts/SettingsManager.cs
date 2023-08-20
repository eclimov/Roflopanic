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

    /***************************/
    public static int experiencePerLevel = 10_000;
    /***************************/

    public static ushort rewardScore = 300;
    public static byte rewardPointsMultiplier = 3;

    private bool isLocaleCoroutineActive = false;

    public static int difficultyId;
    private DifficultyMap[] difficultyMaps;

    public delegate void OnCoinChanceChangedDelegate(int newCoinChance);
    public event OnCoinChanceChangedDelegate OnCoinChanceChange;

    public delegate void OnTotalScoreChangedDelegate(int newScore);
    public event OnTotalScoreChangedDelegate OnTotalScoreChange;

    public delegate void OnHighscoreChangedDelegate(int newScore);
    public event OnHighscoreChangedDelegate OnHighscoreChange;

    public delegate void OnExperienceChangeDelegate(int newVal);
    public event OnExperienceChangeDelegate OnExperienceChange;

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
                totalScore = GetTotalScore(),
                experience = GetExperience(),
                coinChance = GetCoinChance()
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
                    scoreIncrementMultiplier: 17f,
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

    public static int GetCoinChance()
    {
        return PlayerPrefs.GetInt("coinChance", 0);
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
            OnHighscoreChange(newHighscore);
        }

        CloudSaveManager.Instance.Save();
    }

    private void SetTotalScore(int newTotalScore)
    {
        SaveData.totalScore = newTotalScore;
        PlayerPrefs.SetInt("totalScore", newTotalScore);

        if (OnTotalScoreChange != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnTotalScoreChange(newTotalScore);
        }

        CloudSaveManager.Instance.Save();
    }

    public void AddScore(int scoreToAdd)
    {
        SetTotalScore(SaveData.totalScore + scoreToAdd);
    }

    public bool SubtractScore(int scoreToSubtract) // Return true if successfull
    {
        int newScore = SaveData.totalScore - scoreToSubtract;
        if(newScore < 0)
        {
            return false;
        }

        SetTotalScore(newScore);
        return true;
    }

    private void SetCoinChance(int newCoinChance)
    {
        if(newCoinChance > 100) // Prevent having chance value higher than 100
        {
            newCoinChance = 100;
        }

        SaveData.coinChance = newCoinChance;
        PlayerPrefs.SetInt("coinChance", newCoinChance);

        if (OnCoinChanceChange != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnCoinChanceChange(newCoinChance);
        }

        CloudSaveManager.Instance.Save();
    }

    public void AddCoinChance(int coinChanceToAdd)
    {
        SetCoinChance(SaveData.coinChance + coinChanceToAdd);
    }

    public static int GetPlayerLevelNumber()
    {
        return (GetExperience() / experiencePerLevel) + 1;
    }

    public Level GetPlayerLevelByNumber(int number)
    {
        return ProgressionManager.instance.levels[number - 1];
    }

    public static int GetExperience()
    {
        return PlayerPrefs.GetInt("experience", 0);
    }

    private void SetExperience(int newExperience)
    {
        SaveData.experience = newExperience;
        PlayerPrefs.SetInt("experience", newExperience);

        if (OnExperienceChange != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnExperienceChange(newExperience);
        }

        CloudSaveManager.Instance.Save();
    }

    public void AddExperience(int experienceToAdd)
    {
        int experienceCap = experiencePerLevel * (ProgressionManager.instance.levels.Length - 1);

        if ((SaveData.experience + experienceToAdd) > experienceCap)
        {
            SetExperience(experienceCap);
        } else
        {
            SetExperience(SaveData.experience + experienceToAdd);
        }
    }

    public void DeleteData()
    {
        SetHighscore(0);
        SetTotalScore(0);
        SetExperience(0);
        SetCoinChance(0);
    }

    public void SaveSaveData(SaveData.CloudSaveData data)
    {
        SetHighscore(data.highscore);
        SetTotalScore(data.totalScore);
        SetExperience(data.experience);
        SetCoinChance(data.coinChance);
    }
}
