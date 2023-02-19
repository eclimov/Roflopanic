using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] musicClips;

    public AudioSource musicSource;
    public AudioSource soundSource; // TODO: implement for sounds

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

            PlayMusic("music-menu"); // Play menu music by default
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic (string clipName)
    {
        if(musicSource.clip?.name == clipName && musicSource.isPlaying) // If the same music is playing already, do nothing
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

    void Update()
    {
        if(PauseMenu.GameIsPaused)
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
