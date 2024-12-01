using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject panel; 

    // play/transition scene + quit
    public void LoadScene(string game)
    {
        SceneManager.LoadScene(game);
    }

    public void Quit() 
    {
        Application.Quit();
    }

    // open + close options menu
    public void OpenOptions() 
    {
        panel.SetActive(true);
    }

    public void CloseOptions() 
    {
        panel.SetActive(false);
    }

    // pause + unpause
    public void PauseGame() 
    {
        Time.timeScale = 0;
    }

    public void UnpauseGame() 
    {
        Time.timeScale = 1;
    }
}
