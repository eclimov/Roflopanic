using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsInputController : MonoBehaviour
{
    public Toggle toggleMusic;
    public Toggle toggleSound;
    public Toggle toggleVibration;

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();

        toggleMusic.onValueChanged.AddListener(delegate
        {
            FindAnyObjectByType<SettingsManager>().setMusicEnabled(toggleMusic.isOn);
        });

        toggleSound.onValueChanged.AddListener(delegate
        {
            FindAnyObjectByType<SettingsManager>().setSoundEnabled(toggleSound.isOn);
        });

        toggleVibration.onValueChanged.AddListener(delegate
        {
            FindAnyObjectByType<SettingsManager>().setVibrationEnabled(toggleVibration.isOn);
        });
    }

    private void LoadSettings()
    {
        toggleMusic.isOn = SettingsManager.isMusicEnabled;
        toggleSound.isOn = SettingsManager.isSoundEnabled;
        toggleVibration.isOn = SettingsManager.isVibrationEnabled;
    }
}
