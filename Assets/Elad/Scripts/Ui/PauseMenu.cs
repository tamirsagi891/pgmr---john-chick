using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using Managers;
using Nemesh.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Logger = Nemesh.Logger;

public class PauseMenu : MonoBehaviour
{
    public bool gameIsPaused;

    public GameObject pauseMenuUI;

    [SerializeField]
    public LoadSceneManager loadSceneManager;

    private GameObject _firstSelected;


    private void Awake()
    {
        _firstSelected = pauseMenuUI.transform.GetChild(0).gameObject;
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (PlayerStatus.IsGamePause)
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
        PlayerStatus.IsGamePause = false;
    }

    public void ReloadLevel()
    {
        PlayerStatus.IsGamePause = false;
        Time.timeScale = 1f;
        Logger.Log("Reload Level");
        loadSceneManager.ReloadScene();
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_firstSelected);
        Time.timeScale = 0f;
        PlayerStatus.IsGamePause = true;
    }

    public void LoadMenu()
    {
        PlayerStatus.IsGamePause = false;
        Time.timeScale = 1f;
        Logger.Log("load menu");
        loadSceneManager.GoToScene(ScenesHolder.MainMenu);
    }

    public void QuitGame()
    {
        PlayerStatus.IsGamePause = false;
        Logger.Log("quit game");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}