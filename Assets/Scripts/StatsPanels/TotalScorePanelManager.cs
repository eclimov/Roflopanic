using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalScorePanelManager : AbstractScorePanelManager
{
    private void Start()
    {
        score = SettingsManager.SaveData.totalScore;
        UpdateScoreText(score);

        increaseScore = score;

        SettingsManager.instance.OnTotalScoreChange += OnScoreChange;
    }

    protected void OnDestroy()
    {
        SettingsManager.instance.OnTotalScoreChange -= OnScoreChange;
    }
}
