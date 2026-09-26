using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Static singleton instance
    public static AudioManager Instance { get; private set; }

    // Audio clips
    [Header("Sound Effects")]
    [SerializeField] private AudioClip diggitySound;
    [SerializeField] private AudioClip goSound;
    [SerializeField] private AudioClip finishFirstSound;
    [SerializeField] private AudioClip finishOtherSound;
    [SerializeField] private AudioClip digSound;
    
    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.7f;

    [Header("Sound Settings")]
    [SerializeField] private float digSoundCooldown = 0.2f;
    private float lastDigSoundTime = -1f;

    private float defaultSfxPitch = 1.0f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // Update the static instance if this object becomes enabled
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    #region Sound Control Methods

    public void PlayDiggitySound()
    {
        PlaySFX(diggitySound);
    }

    public void PlayGoSound()
    {
        PlaySFX(goSound);
    }

    public void PlayDigSound()
    {
        // Check if enough time has passed since the last dig sound
        if (Time.time - lastDigSoundTime >= digSoundCooldown)
        {
            PlaySFX(digSound);
            lastDigSoundTime = Time.time;
        }
    }

    public void PlayFinishSound(bool isFirst)
    {
        PlaySFX(isFirst ? finishFirstSound : finishOtherSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void SetSFXPitch(float pitch)
    {
        if (sfxSource != null)
        {
            sfxSource.pitch = pitch;
        }
    }

    public void ResetSFXPitch()
    {
        if (sfxSource != null)
        {
            sfxSource.pitch = defaultSfxPitch;
        }
    }

    public void PauseBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (musicSource != null && !musicSource.isPlaying && musicSource.clip != null)
        {
            musicSource.UnPause();
        }
    }

    #endregion
}