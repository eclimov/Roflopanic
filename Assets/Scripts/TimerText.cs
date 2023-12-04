using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimerText : MonoBehaviour
{
    private TMP_Text textComp;

    private float timer; // Seconds
    private int timerSeconds;
    private int timerSecondsDisplayed;

    public delegate void OnTimeoutDelegate();
    public event OnTimeoutDelegate OnTimeout;

    private void Start()
    {
        textComp = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        timerSecondsDisplayed = int.MaxValue; // Initialize and reset time for futher condition
    }

    public bool IsTimerActive()
    {
        return timerSeconds > 0;
    }

    public void SetTimer(int seconds)
    {
        timer = seconds;
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;

            timerSeconds = Mathf.FloorToInt(timer);
            if(timerSeconds >= 0 && timerSeconds < timerSecondsDisplayed)
            {
                SetTimerText(TimeSpan.FromSeconds(timerSeconds).Minutes, TimeSpan.FromSeconds(timerSeconds).Seconds);
                timerSecondsDisplayed = timerSeconds;

                if (timerSecondsDisplayed == 0)
                {
                    if (OnTimeout != null) // It is a MUST to check this, because the event is null if it has no subscribers
                    {
                        OnTimeout();
                    }
                }
            }
        }
    }

    private void SetTimerText(int minutes, int seconds)
    {
        textComp.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
