using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFreezeOverlayPanel : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    private bool fadeIn = false;
    private bool fadeOut = false;

    private void OnEnable()
    {
        canvasGroup.alpha = 0;
        fadeIn = true;
    }

    public void FadeOut()
    {
        fadeOut = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeIn)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime;
                if(canvasGroup.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }

        if(fadeOut)
        {
            fadeIn = false; // Keep it here to prevent cases when fadeIn intersects with fadeOut

            if(canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.unscaledDeltaTime;
                if(canvasGroup.alpha <= 0)
                {
                    fadeOut = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
