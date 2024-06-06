using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly int numPlayers = 2;
    private int turn = 0;
    private int[] score = new int[2];
    [SerializeField] GameObject[] playerPieces = new GameObject[2];
    [SerializeField] GameObject[] playerGhosts = new GameObject[2];

    private readonly int length = 4, width = 4, height = 4;
    private readonly int winLength = 4;
    private int[,,] board;
    private GameObject[,,] pieces;

    public bool gameInPlay { get; private set; }

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

        PieceManager.PieceInPlay += SetGameInPlay;

        GameMenu.BlockInput += SetGameInPlay;
    }

    private void Start()
    {
        HideGhosts();

        board = new int[length, width, height];
        for (int l = 0; l < length; l++)
            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                    board[l, w, h] = -1;

        pieces = new GameObject[length, width, height];

        gameInPlay = true;
    }

    private void OnDisable()
    {
        Column.Selected -= TakeTurn;
        Column.Hover -= ShowGhost;
        Column.Exit -= HideGhosts;

        PieceManager.PieceInPlay -= SetGameInPlay;

        GameMenu.BlockInput -= SetGameInPlay;
    }

    private void Rematch()
    {
        //TODO: delete pieces and reset variables (not score)
    }

    private void TakeTurn(int l, int w, Transform spawnLocation)
    {
        if (LegalMove(l, w))
        {
            HideGhosts();

            int player = turn % numPlayers;
            int h = HighestPieceInColumn(l, w) + 1;

            pieces[l, w, h] = Instantiate(playerPieces[player], spawnLocation.position, Quaternion.Euler(90f, 0f, (float)UnityEngine.Random.Range(0, 360)));
            board[l, w, h] = player;

            if (DidWin(l, w, h))
            {
                
            }
            else if (DidDraw())
            {
                Draw();
            }
            HideGhosts();
            ++turn;
        }
    }

    /*Finds the height of top piece for a given column. 
      Returns -1 if no pieces are in this column.")]*/
    private int HighestPieceInColumn(int l, int w)
    {
        int highestPiece;
        for (highestPiece = 0; highestPiece < height; highestPiece++)
        {
            if (board[l, w, highestPiece] == -1)
            {
                highestPiece--;
                return highestPiece;
            }
        }
        return highestPiece;
    }

    public bool LegalMove(int l, int w)
    {
        if (board[l,w,height-1] == -1)
        {
            return true;
        }
        return false;
    }

    private bool DidWin(int l, int w, int h)
    {
        /*All possible winning lines can be described by following a vector(l, w, h) s.t. l,w,h are elements of { -1, 0, 1}.
The vector (0,0,0) has no direction so can be disregarded
Non zero vectors have a parallel pair going the opposite direction. These lines produce equivalent output from CheckLine()*/
        int[,] possibleLines = new int[13, 3] {{0, 0, 1}, //{0, 0, -1}
                                               {0, 1, 0}, //{0, -1, 0}
                                               {1, 0, 0}, //{-1, 0, 0}
                                               {0, 1, 1}, //{0, -1, -1}
                                               {1, 1, 0}, //{-1, -1, 0}
                                               {1, 0, 1}, //{-1, 0, -1}
                                               {1, 1, 1}, //{-1, -1, -1}
                                               {0, 1, -1}, //{0, -1, 1}
                                               {-1, 1, 0}, //{1, -1, 0}
                                               {1, 0, -1}, //{-1, 0, 1}
                                               {1, 1, -1}, //{-1, -1, 1}
                                               {1, -1, 1}, //{-1, 1, -1}
                                               {-1, 1, 1},}; //{1, -1, -1}
        for (int line = 0; line < possibleLines.GetLength(0); line++)
        {
            int[,] win;
            win = CheckLine(GenerateLine(l, w, h, GetRow(possibleLines, line)), turn%numPlayers);
            if(win != null)
            {
                Win(win);
                return true;
            }
        }

        return false;
    }

    private int[,] GenerateLine(int l, int w, int h, int[] vector)
    {
        int bound = winLength - 1;
        int[,] line = new int[2*bound+1,3];

        for(int posMult = -bound; posMult <= bound; posMult++)
        {
                line[posMult + bound, 0] = l + vector[0] * posMult;
                line[posMult + bound, 1] = w + vector[1] * posMult;
                line[posMult + bound, 2] = h + vector[2] * posMult;
        }
        return line;
    }

    private int[,] CheckLine(int[,] possibleConnections, int player)
    {
        int consecutive = 0;
        for (int e = 0; e < possibleConnections.GetLength(0); e++)
        {
            int piece;
            try
            {
                int[] piecePos = GetRow(possibleConnections, e);
                piece = board[piecePos[0], piecePos[1], piecePos[2]];
            }
            catch
            {
                piece = -1;
            }

            if (piece == player)
            {
                consecutive++;
            }
            else consecutive = 0;
            if (consecutive == winLength)
            {
                int[,] win = new int[winLength, 3];
                for (int i = 0; i < winLength; i++)
                {
                    int[] pos = GetRow(possibleConnections, e - i);
                    win[i, 0] = pos[0];
                    win[i, 1] = pos[1];
                    win[i, 2] = pos[2];
                }
                return win;
            }
        }

        return null;
    }

    private bool DidDraw()
    {
        if (turn >= length*width*height - 1)
        {
            return true;
        }
        return false;
    }

    private void Win(int[,] win)
    {
        int player = turn % numPlayers;
        for (int l = 0; l < length; l++)
            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                        pieces[l, w, h]?.GetComponent<PieceManager>().Hide();

        for (int i = 0; i < winLength; i++)
        {
            pieces[win[i, 0], win[i, 1], win[i, 2]].GetComponent<PieceManager>().Highlight();
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
        if (LegalMove(l, w))
        {
            int player = turn % numPlayers;
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
        gameInPlay = !inPlay;
    }
    private int[] GetRow(int[,] array, int index)
    {
        int[] row = new int[array.GetLength(0)];
        for (int i = 0; i < array.GetLength(1); ++i)
        {
            row[i] = array[index,i];
        }
        return row;
    }

    public string BoardStateToString()
    {
        string text = "";
        for (int l = 0; l < length; l++)
        {
            for (int w = 0; w < width; w++)
            {
                text += "Column:" + l.ToString() + "," + w.ToString() + "  ";
                for (int h = 0; h < height; h++)
                {
                    text += board[l, w, h].ToString() + ",";
                }
                text += "\n";
            }
            text += "\n";
        }

        return text;
    }
}
