using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscorePanelManager : AbstractScorePanelManager
{
    private void Start()
    {
        score = SettingsManager.SaveData.highscore;
        UpdateScoreText(score);

        increaseScore = score;

        SettingsManager.instance.OnHighscoreChange += OnScoreChange;
    }

    protected void OnDestroy()
    {
        SettingsManager.instance.OnHighscoreChange -= OnScoreChange;
    }
}
