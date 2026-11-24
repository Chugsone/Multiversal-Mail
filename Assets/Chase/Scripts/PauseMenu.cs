using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool _isPaused;
    [SerializeField] private GameObject _pauseMenu;

    // Pause game
    public void Pause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return; 
        _isPaused = true;
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    // Resume game
    public void Resume(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        _isPaused = false;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Toggle pause state
    public void Toggle(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        _isPaused = !_isPaused;
        _pauseMenu.SetActive(_isPaused); 
        Time.timeScale = _isPaused ? 0f : 1f; 
    }

    // Quit to title screen
    public void Quit()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Title");
    }

    // Button To resume incase monkey brain player
    public void ResumeButton()
    {
        _isPaused = false;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
