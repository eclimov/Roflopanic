using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<ProgressBar>().AnimateProgress(SettingsManager.SaveData.totalScore, .5f);
    }
}
