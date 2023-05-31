using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Logger = Nemesh.Logger;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPause()
    {
         
        if (GameIsPaused)
        {
            Resume();
        }

        else
        {
            Pause();
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
        Logger.Log("load menu");
        
    }
    
    public void QuitGame()
    {
        Logger.Log("quit game");
    }
}
