using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 

/// <summary>
/// Controls Scene loading and opening options within the main menu.
/// </summary>
public class Menu : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public GameObject errorText; 
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

    public void PlayGame(string game) 
    {
        string input = usernameInput.text;
        // Debug.Log(input); 
        if (input == "" && PlayerPrefs.GetString("Username") == "") {
            errorText.SetActive(true); 
        }
        else {
            PlayerPrefs.SetString("Username", input);  
            PlayerPrefs.Save(); 
            LoadScene(game);
        }
    }
}
