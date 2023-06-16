using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] musicClips;

    public AudioClip soundButton;
    public AudioClip soundDeath;
    public AudioClip soundCash;
    public AudioClip soundCoin;
    public AudioClip soundVoiceLetter;
    public AudioClip soundFanfare;

    public AudioSource musicSource;
    public AudioSource soundSource;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

            musicSource.loop = true;
            soundSource.loop = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadMusicSettings();
    }

    public void LoadMusicSettings()
    {
        if (SettingsManager.isMusicEnabled)
        {
            PlayMusic("music-menu"); // Play menu music by default
        } else if(musicSource.isPlaying) // if music is disabled, but it's currently playing, stop it
        {
            musicSource.Stop();
        }
    }

    public void PlayMusic (string clipName)
    {
        if(
            !SettingsManager.isMusicEnabled || // If music disabled OR
            musicSource.clip?.name == clipName && musicSource.isPlaying // If the same music is playing already
        )
        {
            return;
        }

        AudioClip clip = Array.Find(musicClips, item => item.name == clipName);
        if(clip == null)
        {
            Debug.LogWarning("Music: " + clipName + " not found!");
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if(musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void UnpauseMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    public void PlayButtonSound()
    {
        PlaySound(soundButton);
    }

    public void PlayDeathSound()
    {
        PlaySound(soundDeath);
    }

    public void PlayCashSound()
    {
        PlaySound(soundCash);
    }

    public void PlayCoinSound()
    {
        PlaySound(soundCoin);
    }

    public void PlayVoiceLetterSound()
    {
        PlaySound(soundVoiceLetter);
    }

    public void PlayFanfareSound()
    {
        PlaySound(soundFanfare);
    }

    public void PlaySound(AudioClip clip)
    {
        if(!SettingsManager.isSoundEnabled)
        {
            return;
        }

        soundSource.PlayOneShot(clip);
    }

    void Update()
    {
        // TODO: rework this to avoid checking the condition each frame
        if (PauseMenu.GameIsPaused)
        {
            musicSource.pitch = .6f;
            musicSource.volume = .6f;
        } else
        {
            musicSource.pitch = 1f;
            musicSource.volume = 1f;
        }
    }
}
