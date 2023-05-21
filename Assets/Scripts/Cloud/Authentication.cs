using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

public class Authentication : MonoBehaviour
{
    public static bool Authenticated { get; private set; }

    public static PlayGamesPlatform Platform { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if(!Authenticated) // Prevent re-authenticating and overwriting existing data on Menu load
        {
            Login();
        }
    }

    private void Login()
    {
        if(Platform == null)
        {
            Platform = BuildPlatform();
        }

        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            Authenticated = success;
            OnAuthenticationSucceeded();
        });
    }

    private void OnAuthenticationSucceeded()
    {
        if(Authenticated)
        {
            CloudSaveManager.Instance.Load();
        } else
        {
            CloudSaveManager.Instance.UseLocalData();
        }
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
