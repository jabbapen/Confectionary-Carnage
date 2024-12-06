using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 

/// <summary>
/// Manages audio settings for the game, including music and sound effects (SFX) volume.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager manager;
    public AudioMixer mixer;

    private float musicVol = 1f;
    private float sfxVol = 1f;

    void Awake()
    {
        if (manager == null)
        {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadSettings();
    }

    /// <summary>
    /// Sets the global music volume and updates the corresponding AudioMixer parameter.
    /// </summary>
    /// <param name="vol">The desired music volume level (range 0 to 1).</param>
    public void SetGMusicVol(float vol)
    {
        musicVol = vol;
        PlayerPrefs.SetFloat("MusicVolume", vol);
        mixer.SetFloat("Music", Mathf.Log10(vol) * 20);
    }

    /// <summary>
    /// Sets the global sound effects (SFX) volume and updates the corresponding AudioMixer parameter.
    /// </summary>
    /// <param name="vol">The desired SFX volume level (range 0 to 1).</param>
    public void SetGSFXVol(float vol)
    {
        sfxVol = vol;
        PlayerPrefs.SetFloat("SFXVolume", vol);
        mixer.SetFloat("SFX", Mathf.Log10(vol) * 20);
    }

    /// <summary>
    /// Loads the saved music and SFX volume settings from PlayerPrefs and updates the AudioMixer.
    /// </summary>
    public void LoadSettings()
    {
        musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mixer.SetFloat("Music", Mathf.Log10(musicVol) * 20);
        mixer.SetFloat("SFX", Mathf.Log10(sfxVol) * 20);
    }

    /// <summary>
    /// Updates the UI sliders to reflect the current music and SFX volume levels.
    /// </summary>
    /// <param name="musicSlider">The UI slider for adjusting music volume.</param>
    /// <param name="sfxSlider">The UI slider for adjusting SFX volume.</param>
    public void UpdateSliders(Slider musicSlider, Slider sfxSlider)
    {
        if (musicSlider != null)
        {
            musicSlider.value = musicVol;
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVol;
        }
    }
}
