using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;
using Unity.VisualScripting;

public class Authentication : MonoBehaviour
{
    public GameObject loadingOverlay; // Disabled by default
    public GameObject adBanner; // Disabled by default

    public static bool Authenticated { get; private set; }

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

        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                Authenticated = true;
                OnAuthenticationSucceeded();
            }
            else
            {
                Debug.LogError("Failed to authenticate: " + status);
                Authenticated = false;
                HideLoadingOverlay();
            }
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
}
