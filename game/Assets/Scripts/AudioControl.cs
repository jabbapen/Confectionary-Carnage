using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioControl : MonoBehaviour {
    public AudioMixer mixer; 
    public void SetMusicVolume (float sliderValue)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
        mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume (float sliderValue)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        mixer.SetFloat("SFX", Mathf.Log10(sliderValue) * 20);
    }

    public void PlaySound(AudioSource sound) {
        sound.Play();
    }
}
