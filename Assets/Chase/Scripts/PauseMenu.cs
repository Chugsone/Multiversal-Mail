using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused;

    [SerializeField] private GameObject pauseMenu;
    public void Pause(InputAction.CallbackContext ctx)
    {

        isPaused = true;
        pauseMenu.SetActive(true);

        Time.timeScale = 0;
    }

    private static void SetActive(bool v)
    {
        throw new NotImplementedException();
    }

    public void Resume(InputAction.CallbackContext ctx)
    {
        isPaused = false;
        pauseMenu.SetActive(false);

        Time.timeScale = 1;
    }

    public void ResumeButt()
    {
        isPaused = false;
        pauseMenu.SetActive(false);

        Time.timeScale = 1;
    }

    public void Toggle(InputAction.CallbackContext ctx)
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        Time.timeScale = isPaused ? 0 : 1;
    }
}
