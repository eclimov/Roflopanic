using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;

public class Authentication : MonoBehaviour
{
    public GameObject loadingOverlay; // Disabled by default
    public GameObject adBanner; // Disabled by default

    public static bool Authenticated { get; private set; }

    public static PlayGamesPlatform Platform { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (!Authenticated) // Prevent re-authenticating and overwriting existing data on Menu load
        {
            Login();
        } else
        {
            ShowAdBanner();
        }
    }

    private void Login()
    {
        ShowLoadingOverlay();

        if (Platform == null)
        {
            try
            {
                Platform = BuildPlatform();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                HideLoadingOverlay();
            }
        }

        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            Authenticated = success;
            OnAuthenticationSucceeded();
        });
    }

    private void OnAuthenticationSucceeded()
    {
        CloudSaveManager.Instance.OnSaveLoaded += HideLoadingOverlay;

        if (Authenticated)
        {
            CloudSaveManager.Instance.Load();
        } else
        {
            CloudSaveManager.Instance.UseLocalData();
        }
    }

    private void ShowLoadingOverlay()
    {
        loadingOverlay.SetActive(true);
    }

    private void HideLoadingOverlay()
    {
        loadingOverlay.SetActive(false);
        CloudSaveManager.Instance.OnSaveLoaded -= HideLoadingOverlay;

        ShowAdBanner();
    }

    private void ShowAdBanner()
    {
        adBanner.SetActive(true);
    }

    private PlayGamesPlatform BuildPlatform()
    {
        var builder = new PlayGamesClientConfiguration.Builder();
        builder.EnableSavedGames();
        // builder.RequestServerAuthCode(false); // Uncomment this if using auth via Firebase

        PlayGamesPlatform.InitializeInstance(builder.Build());
        PlayGamesPlatform.DebugLogEnabled = true;

        return PlayGamesPlatform.Activate();
    }
}
