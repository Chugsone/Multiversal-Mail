using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Start");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SettingsMenu()
    {
        SceneManager.LoadScene("Settings");
    }
    public void CreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }

}
