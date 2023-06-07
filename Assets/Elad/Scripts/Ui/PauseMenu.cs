using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Logger = Nemesh.Logger;

public class PauseMenu : MonoBehaviour
{
    public bool gameIsPaused;

    public GameObject pauseMenuUI;

    public bool GameIsPaused
    {
        get => gameIsPaused;
        set
        {
            gameIsPaused = value;
            PlayerStatus.isGamePause = value;
        }
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            print(context.phase);
            if (GameIsPaused)
            {
                Resume();
            }

            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        Logger.Log("load menu");
        SceneManager.LoadScene(SceneNamesStrings.menuScene);
    }

    public void QuitGame()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        Logger.Log("quit game");
        Application.Quit();
    }
}