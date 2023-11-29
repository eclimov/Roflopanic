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
            difficultyManager.OnIsDifficultyChange += SetScrollingSpeed;
        }
    }

    protected void OnDestroy()
    {
        if (difficultyManager != null) // If it's the main menu
        {
            difficultyManager.OnIsDifficultyChange -= SetScrollingSpeed;
        }
    }

    protected override void SetScrollingSpeed()
    {
        float backgroundSpeed;

        switch (SettingsManager.difficultyId)
        {
            case 1: // Medium
                backgroundSpeed = .07f;
                break;
            case 2: // Hardcore
                backgroundSpeed = .14f;
                break;
            default: // Easy
                backgroundSpeed = .01f;
                break;
        }


        scrolling = new Vector2(backgroundSpeed, 0f);
    }
}
