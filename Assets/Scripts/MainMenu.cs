using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
        Debug.LogWarning("Application Quit");
    }
}
