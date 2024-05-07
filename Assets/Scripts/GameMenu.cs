using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameManager gm;
    public GameObject winScreen;
    public TMP_Text winText;

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        winScreen.SetActive(false);
    }

    public void Winner(int player)
    {
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
