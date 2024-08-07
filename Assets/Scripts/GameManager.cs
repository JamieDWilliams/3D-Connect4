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
    private int firstPlayer;

    [SerializeField] GameObject[] playerPieces = new GameObject[2];
    [SerializeField] GameObject[] playerGhosts = new GameObject[2];

    public int[] score { get; private set; }

    public bool GamePaused { get; private set; }

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

        Piece.PieceInPlay += SetGamePaused;

        GameMenu.BlockInput += SetGamePaused;
        GameMenu.Rematch += Rematch;
        GameMenu.ClearScore += ClearScore;

        Board.WinningLine += Win;
        Board.Draw += Draw;
    }

    private void Start()
    {
        HideGhosts();

        firstPlayer = 0;
        board = new Board(firstPlayer);
        score = new int[board.NumPlayers];

        pieces = new GameObject[board.Length, board.Width, board.Height];
        GamePaused = false;
    }

    private void OnDisable()
    {
        Column.Selected -= TakeTurn;
        Column.Hover -= ShowGhost;
        Column.Exit -= HideGhosts;

        Piece.PieceInPlay -= SetGamePaused;

        GameMenu.BlockInput -= SetGamePaused;
        GameMenu.Rematch -= Rematch;
        GameMenu.ClearScore -= ClearScore;

        Board.WinningLine -= Win;
        Board.Draw -= Draw;
    }
    private void TakeTurn(int l, int w, Transform spawnLocation)
    {
        if (board.LegalMove(l, w) && !GamePaused)
        {
            int player = board.CurrentPlayer();

            pieces[l, w, board.HighestPieceInColumn(l, w)+1] = Instantiate(playerPieces[player], spawnLocation.position, Quaternion.Euler(90f, 0f, (float)UnityEngine.Random.Range(0, 360)));
            board.TakeTurn(l, w);
        }
    }

    private void Rematch()
    {
        foreach(GameObject piece in pieces){
            Destroy(piece);
        }

        board = new Board(firstPlayer);

        pieces = new GameObject[board.Length, board.Width, board.Height];
        
        GamePaused = false;
        Piece.PieceInPlay += SetGamePaused;
    }

    private void Win(int[,] win)
    {
        Piece.PieceInPlay -= SetGamePaused;
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

    private void ClearScore(){
        score = new int[board.NumPlayers];
    }

    private void SetGamePaused(bool paused)
    {
        GamePaused = paused;
    }

    public void SetFirstPlayer(int firstPlayer){
        this.firstPlayer = firstPlayer;
    }
    
    public bool ValidMove(int l, int w)
    {
        if(board.LegalMove(l, w) && !GamePaused)
        {
            return true;
        }
        return false;
    }
    
}
