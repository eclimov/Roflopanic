using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public GameObject countdownNumberPrefab;
    public AudioClip countdownTickSound;

    private float countdown;
    private int displayedNumber = int.MaxValue; // It should be bigger than countdown by default, for the 1st iteration to work
    private int currentNumber;

    private Color32 color;

    public delegate void OnTickDelegate(int timeRemains);
    public event OnTickDelegate OnTick;

    public void SetCountdownTime(float timeSeconds)
    {
        countdown = timeSeconds;
    }

    public void Stop()
    {
        countdown = 0f;
    }

    public bool IsActive()
    {
        return countdown > 0;
    }

    private void DisplayNumber(int number)
    {
        if (currentNumber < 4)
        {
            AudioManager.instance.PlaySound(countdownTickSound);
        }

        CountdownNumber countdownNumber = Instantiate(countdownNumberPrefab, GameObject.Find("Canvas").transform).GetComponent<CountdownNumber>();
        countdownNumber.SetText(number.ToString());
        switch(number)
        {
            case 3:
                color = new Color32(255, 216, 52, 255); // Yellow
                break;
            case 2:
                color = new Color32(255, 135, 52, 255); // Orange
                break;
            case 1:
            case 0:
                color = new Color32(255, 52, 52, 255); // Red
                break;
            default:
                color = new Color32(161, 255, 52, 255); // Green
                break;
        }
        countdownNumber.SetColor(color);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsActive())
        {
            countdown -= Time.deltaTime;
            currentNumber = Mathf.CeilToInt(countdown);

            if (currentNumber != displayedNumber) // To make sure to call once
            {
                if (currentNumber > 0)
                {
                    DisplayNumber(currentNumber);
                    displayedNumber = currentNumber;
                }

                if (OnTick != null) // It is a MUST to check this, because the event is null if it has no subscribers
                {
                    OnTick(currentNumber);
                }
            }
        }
    }
}
