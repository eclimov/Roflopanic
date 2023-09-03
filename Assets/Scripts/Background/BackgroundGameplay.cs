using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGameplay : AbstrctBackground
{
    protected override void SetScrolling()
    {
        scrolling = new Vector2(SettingsManager.instance.GetDifficultyMap().backgroundSpeed, 0f);
    }

    public void NegateScrolling()
    {
        scrolling = -scrolling;
    }
}
