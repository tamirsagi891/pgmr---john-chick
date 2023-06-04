using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Nemesh.Logger;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneNamesStrings.firstLevel);
    }
    
    public void QuitGame()
    {
        Logger.Log("Quit game");
        Application.Quit();
    }
}
