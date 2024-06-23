using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] TMP_Text winText;
    [SerializeField] TMP_Text player1Score;
    [SerializeField] TMP_Text player2Score;

    public static event Action<bool> BlockInput; //TODO: piece manager enables input again after
    public static event Action Rematch;
    public static event Action ClearScore;

    private void OnEnable()
    {
        GameManager.GameOver += Winner;
    }
    private void Start()
    {
        Close();
    }
    private void OnDisable()
    {
        GameManager.GameOver -= Winner;
    }

    private void Close()
    {
        winScreen.SetActive(false);
    }

    private void Winner(int player)
    {
        BlockInput(true);

        if (player == -1)
        {
            winText.text = "Draw!";
        }
        else
        {
            winText.text = $"Player {player} Wins!";
        }

        player1Score.text = GameManager.Instance.score[0].ToString();
        player2Score.text = GameManager.Instance.score[1].ToString();

        winScreen.SetActive(true);
    }

    public void RestartButton()
    {
        Rematch();
        ClearScore();
        Close();
    }

    public void RematchButton()
    {
        Rematch();
        Close();
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
