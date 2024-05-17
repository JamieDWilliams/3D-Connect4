using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject winScreen;
    public TMP_Text winText;

    public static event Action<bool> BlockInput; //TODO: piece manager enables input again after

    private void OnEnable()
    {
        GameManager.GameOver += Winner;
    }
    private void Start()
    {
        Refresh();
    }
    private void OnDisable()
    {
        GameManager.GameOver -= Winner;
    }

    private void Refresh()
    {
        winScreen.SetActive(false);
    }

    //TODO: Add Score text
    public void Winner(int player)
    {
        BlockInput(true);
        //Refresh();

        if (player == -1)
        {
            winText.text = "Draw!";
        }
        else
        {
            winText.text = $"Player {player} Wins!";
        }

        winScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        //TODO: call refresh and wipe score instead of reloading game.
    }

    public void Rematch()
    {
        Refresh();
        Debug.LogWarning("Unfinished");
        //TODO: track score and clean the board for another round
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
