using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMenu : AbstrctBackground
{
    private DifficultyManager difficultyManager;

    // Start is called before the first frame update
    void Start()
    {
        difficultyManager = FindObjectOfType<DifficultyManager>();

        if(difficultyManager != null) // If it's the main menu
        {
            difficultyManager.OnIsDifficultyChange += SetScrolling;
        }
    }

    protected void OnDestroy()
    {
        if (difficultyManager != null) // If it's the main menu
        {
            difficultyManager.OnIsDifficultyChange -= SetScrolling;
        }
    }

    protected override void SetScrolling()
    {
        float backgroundSpeed;

        switch (SettingsManager.difficultyId)
        {
            case 0:
                backgroundSpeed = .01f;
                break;
            case 1:
                backgroundSpeed = .05f;
                break;
            case 2:
                backgroundSpeed = .12f;
                break;
            default:
                backgroundSpeed = .01f;
                break;
        }


        scrolling = new Vector2(backgroundSpeed, 0f);
    }
}
