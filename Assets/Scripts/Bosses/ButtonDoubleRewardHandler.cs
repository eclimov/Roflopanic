using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonDoubleRewardHandler : MonoBehaviour
{
    public BossFightConfirmManager bossFightConfirmManager;
    public GameObject loadingCircle;
    public Image buttonImage;
    public TMP_Text buttonText;

    private RewardedAdManager rewardedAdManager;
    private Button button;

    private bool isRewardReceived;

    // Start is called before the first frame update
    void Start()
    {
        rewardedAdManager = FindAnyObjectByType<RewardedAdManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        OnIsAdReadyChangeHandler(rewardedAdManager.IsAdReady);
        rewardedAdManager.OnIsAdReadyChange += OnIsAdReadyChangeHandler;
        rewardedAdManager.OnAdRewardGranted += OnAdRewardGrantedHandler;
    }

    protected void OnDestroy()
    {
        rewardedAdManager.OnIsAdReadyChange -= OnIsAdReadyChangeHandler;
        rewardedAdManager.OnAdRewardGranted -= OnAdRewardGrantedHandler;
    }

    private void OnClick()
    {
        rewardedAdManager.ShowRewardedAd();
    }

    private void OnAdRewardGrantedHandler()
    {
        Time.timeScale = 0f; // Prevent unpausing by ADs API

        isRewardReceived = true;

        loadingCircle.SetActive(false);
        ActivateButton(false);

        StartCoroutine(bossFightConfirmManager.DoubleDown());
    }

    private void OnIsAdReadyChangeHandler(bool status)
    {
        if (isRewardReceived) return;

        ActivateButton(status);
        loadingCircle.SetActive(!status);
    }

    private void ActivateButton(bool status)
    {
        button.interactable = status;

        byte alpha = (byte)(status ? 255 : 150);

        buttonImage.color = new Color32(255, 255, 255, alpha);
        buttonText.color = new Color32(255, 202, 150, alpha);
    }
}
