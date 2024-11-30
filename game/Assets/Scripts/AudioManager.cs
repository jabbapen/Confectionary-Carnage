using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 

public class AudioManager : MonoBehaviour {

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
    public void SetGMusicVol(float vol)
    {
        musicVol = vol;
        PlayerPrefs.SetFloat("MusicVolume", vol);
        mixer.SetFloat("Music", Mathf.Log10(vol) * 20);
    }

    public void SetGSFXVol(float vol)
    {
        sfxVol = vol;
        PlayerPrefs.SetFloat("SFXVolume", vol);
        mixer.SetFloat("SFX", Mathf.Log10(vol) * 20);
    }

    public void LoadSettings()
    {
        musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mixer.SetFloat("Music", Mathf.Log10(musicVol) * 20);
        mixer.SetFloat("SFX", Mathf.Log10(sfxVol) * 20);
    }

    public void UpdateSliders(Slider musicSlider, Slider sfxSlider)
    {
        if (musicSlider!= null)
        {
            musicSlider.value = musicVol;
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVol;
        }
    }
}
