using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour //TODO: implement correct logic with new board class. improve events etc
{
    public static GameManager Instance { get; private set; }

    private Board board;
    private GameObject[,,] pieces;

    [SerializeField] GameObject[] playerPieces = new GameObject[2];
    [SerializeField] GameObject[] playerGhosts = new GameObject[2];

    private int[] score;

    public bool GameInPlay { get; private set; }

    public static event Action<int> GameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        Column.Selected += TakeTurn;
        Column.Hover += ShowGhost;
        Column.Exit += HideGhosts;

        Piece.PieceInPlay += SetGameInPlay;

        GameMenu.BlockInput += SetGameInPlay;

        Board.WinningLine += Win;
        Board.Draw += Draw;
    }

    private void Start()
    {
        HideGhosts();

        //ugly code - all values for the board
        int[] lwh = { 4, 4, 4 };
        int players = 2;
        int winLength = 4;
        board = new Board(lwh, winLength, players);
        score = new int[board.NumPlayers];

        pieces = new GameObject[board.Length, board.Width, board.Height];
        GameInPlay = true;
    }

    private void OnDisable()
    {
        Column.Selected -= TakeTurn;
        Column.Hover -= ShowGhost;
        Column.Exit -= HideGhosts;

        Piece.PieceInPlay -= SetGameInPlay;

        GameMenu.BlockInput -= SetGameInPlay;

        Board.WinningLine -= Win;
        Board.Draw -= Draw;
    }
    private void TakeTurn(int l, int w, Transform spawnLocation)
    {
        if (board.LegalMove(l, w))
        {
            int player = board.CurrentPlayer();

            pieces[l, w, board.HighestPieceInColumn(l, w)+1] = Instantiate(playerPieces[player], spawnLocation.position, Quaternion.Euler(90f, 0f, (float)UnityEngine.Random.Range(0, 360)));
            board.TakeTurn(l, w);
        }
    }

    private void Rematch()
    {
        //TODO: delete pieces and reset variables (not score)
    }

    private void Win(int[,] win)
    {
        int player = board.CurrentPlayer();
        for (int l = 0; l < board.Length; l++)
            for (int w = 0; w < board.Width; w++)
                for (int h = 0; h < board.Height; h++)
                        pieces[l, w, h]?.GetComponent<Piece>().Hide();

        for (int i = 0; i < board.WinLength; i++)
        {
            pieces[win[i, 0], win[i, 1], win[i, 2]].GetComponent<Piece>().Highlight();
        }

        ++score[player];
        GameOver(player + 1);
    }

    private void Draw()
    {
        Debug.Log("Draw");
        GameOver(-1);
    }

    private void ShowGhost(int l, int w, Transform location)
    {
        if (board.LegalMove(l, w))
        {
            int player = board.CurrentPlayer();
            GameObject ghost = playerGhosts[player];

            ghost.SetActive(true);
            ghost.transform.position = location.position;
        }
    }

    private void HideGhosts()
    {
        foreach(GameObject ghost in playerGhosts)
        {
            ghost.SetActive(false);
        }
    }

    private void SetGameInPlay(bool inPlay)
    {
        GameInPlay = !inPlay;
    }
    
    public bool ValidMove(int l, int w)
    {
        if(board.LegalMove(l, w) && GameInPlay)
        {
            return true;
        }
        return false;
    }
    
}
