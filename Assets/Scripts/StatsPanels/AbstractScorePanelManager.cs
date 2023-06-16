using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class AbstractScorePanelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComp;

    protected int score = 0;
    protected int targetScore = 0;

    protected float increaseScore;
    protected bool isSmoothIncrement;

    protected void UpdateScoreText(int value)
    {
        textComp.text = value.ToString();
    }

    private void Update()
    {
        if (isSmoothIncrement)
        {
            if (targetScore != score)
            {
                increaseScore = Mathf.MoveTowards(increaseScore, targetScore, Time.deltaTime * SettingsManager.rewardScore);
                score = (int)increaseScore;
                UpdateScoreText(score);
            }
            else
            {
                isSmoothIncrement = false;
            }
        }
    }

    protected void OnScoreChange(int newScore)
    {
        targetScore = newScore;
        isSmoothIncrement = true;
    }
}
