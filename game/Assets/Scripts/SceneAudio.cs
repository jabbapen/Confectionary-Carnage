using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class SceneAudio : MonoBehaviour
{
    private AudioManager audioManager;
    public Slider musicSlider;
    public Slider sfxSlider; 

    void Start()
    {
        audioManager = AudioManager.manager;
        if (audioManager != null)
        {
            audioManager.LoadSettings(); 
            audioManager.UpdateSliders(musicSlider, sfxSlider); 
        }
    }
    public void SetMusicVol(float vol)
    {
        audioManager.SetGMusicVol(vol);
    }

    public void SetSFXVol(float vol)
    {
        audioManager.SetGSFXVol(vol); 
    }

    public void PlaySound(AudioSource sound) 
    {
        sound.Play();
    }

}
