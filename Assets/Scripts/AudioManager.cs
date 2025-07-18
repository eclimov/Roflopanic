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
    public AudioClip soundPartyHorn;
    public AudioClip soundButtonLips;
    public AudioClip soundOpenBag;
    public AudioClip soundBallBounce;
    public AudioClip soundAxeHitWoodAttempt;
    public AudioClip soundAxeHitWoodSuccessful;
    public AudioClip soundSwoosh;

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

    public void LoadMusicSettings()
    {
        if (SettingsManager.isMusicEnabled)
        {
            PlayMusicMenu(); // Play menu music by default
        } else if(musicSource.isPlaying) // if music is disabled, but it's currently playing, stop it
        {
            musicSource.Stop();
        }
    }

    public void PlayMusicMenu()
    {
        string clipName = "music-menu";
        if(SettingsManager.IsChristmasTime())
        {
            clipName = UnityEngine.Random.Range(0, 2) == 0 ? "music-reed-flutes" : "music-dance-of-the-sugar-plum-fairy";
        }

        PlayMusic(clipName);
    }

    public void PlayMusic (string clipName)
    {
        if(musicSource.clip?.name == clipName && musicSource.isPlaying) // If the same music is playing already
        {
            return;
        }

        AudioClip clip = Array.Find(musicClips, item => item.name == clipName);
        if(clip == null)
        {
            Debug.LogWarning("Music: " + clipName + " not found!");
            return;
        }

        PlayMusic(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!SettingsManager.isMusicEnabled) // If music disabled
        {
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

    public void PlayPartyHornSound()
    {
        PlaySound(soundPartyHorn);
    }

    public void PlayButtonLipsSound()
    {
        PlaySound(soundButtonLips);
    }

    public void PlayOpenBagSound()
    {
        PlaySound(soundOpenBag);
    }

    public void AxeHitWoodAttemptSound()
    {
        PlaySound(soundAxeHitWoodAttempt);
    }

    public void AxeHitWoodSuccessfulSound()
    {
        PlaySound(soundAxeHitWoodSuccessful);
    }

    public void PlayBallBounceSound()
    {
        PlaySound(soundBallBounce);
    }

    public void PlaySwooshSound()
    {
        PlaySound(soundSwoosh);
    }

    public void PlaySound(AudioClip clip)
    {
        if(!SettingsManager.isSoundEnabled)
        {
            return;
        }

        soundSource.PlayOneShot(clip);
    }

    public void StopPlayingSounds()
    {
        soundSource.Stop();
    }

    public void SetSlowMotion(bool status)
    {
        float value = status ? .6f : 1f;

        musicSource.pitch = value;
        musicSource.volume = value;
    }
}
