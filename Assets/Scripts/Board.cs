using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Board 
{
    public int Length { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int WinLength { get; private set; }
    public int NumPlayers { get; private set; }

    private int[,,] boardState;
    private int turn = 0;
    private int firstPlayer;
    public static event Action<int[,]> WinningLine;
    public static event Action Draw;

    public Board(){
        Length = 4; Width = 4; Height = 4;

        boardState = new int[Length, Width, Height];
        for (int l = 0; l < Length; l++)
            for (int w = 0; w < Width; w++)
                for (int h = 0; h < Height; h++)
                    boardState[l, w, h] = -1;

        NumPlayers = 2;
        WinLength = 4;
        firstPlayer = 0;
    }
    public Board(int firstPlayer){
        Length = 4; Width = 4; Height = 4;

        boardState = new int[Length, Width, Height];
        for (int l = 0; l < Length; l++)
            for (int w = 0; w < Width; w++)
                for (int h = 0; h < Height; h++)
                    boardState[l, w, h] = -1;

        NumPlayers = 2;
        WinLength = 4;
        this.firstPlayer = firstPlayer;
    }
    public Board(int[] lwh, int winLength, int numPlayers, int firstPlayer) {
        Length = lwh[0]; Width = lwh[1]; Height = lwh[2];

        boardState = new int[Length, Width, Height];
        for (int l = 0; l < Length; l++)
            for (int w = 0; w < Width; w++)
                for (int h = 0; h < Height; h++)
                    boardState[l, w, h] = -1;

        NumPlayers = numPlayers;
        WinLength = winLength;
        this.firstPlayer = firstPlayer;
    }

    public void TakeTurn(int l, int w)
    {
        int h = HighestPieceInColumn(l, w) + 1;

        boardState[l, w, h] = CurrentPlayer();
        

        int[,] winningLine = DidWin(l, w, h);
        if (winningLine != null)
        {
            WinningLine(winningLine);
        }
        else if (DidDraw())
        {
            Draw();
        }
        else
        {
            turn++;
        };
    }
    
    private int[,] DidWin(int l, int w, int h)
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
        int[,] win = null;
        for (int line = 0; line < possibleLines.GetLength(0); line++)
        {
            win = CheckLine(GenerateLine(l, w, h, GetRow(possibleLines, line)), CurrentPlayer());
            if (win != null) break;
        }
        return win;
    }

    public bool DidDraw()
    {
        if (turn >= Length * Width * Height - 1)
        {
            return true;
        }
        return false;
    }

    /*Finds the height of top piece for a given column. 
  Returns -1 if no pieces are in this column.*/
    public int HighestPieceInColumn(int l, int w)
    {
        int highestPiece;
        for (highestPiece = 0; highestPiece < Height; highestPiece++)
        {
            if (boardState[l, w, highestPiece] == -1)
            {
                highestPiece--;
                return highestPiece;
            }
        }
        return highestPiece;
    }

    public bool LegalMove(int l, int w)
    {
        if (boardState[l, w, Height - 1] == -1)
        {
            return true;
        }
        return false;
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
                piece = boardState[piecePos[0], piecePos[1], piecePos[2]];
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
            if (consecutive == WinLength)
            {
                int[,] win = new int[WinLength, 3];
                for (int i = 0; i < WinLength; i++)
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

    private int[,] GenerateLine(int l, int w, int h, int[] vector)
    {
        int bound = WinLength - 1;
        int[,] line = new int[2 * bound + 1, 3];

        for (int posMult = -bound; posMult <= bound; posMult++)
        {
            line[posMult + bound, 0] = l + vector[0] * posMult;
            line[posMult + bound, 1] = w + vector[1] * posMult;
            line[posMult + bound, 2] = h + vector[2] * posMult;
        }
        return line;
    }

    private int[] GetRow(int[,] array, int index)
    {
        int[] row = new int[array.GetLength(0)];
        for (int i = 0; i < array.GetLength(1); ++i)
        {
            row[i] = array[index, i];
        }
        return row;
    }

    public int CurrentPlayer()
    {
        return (turn+firstPlayer) % NumPlayers;
    }

    public string BoardStateToString()
    {
        string text = "";
        for (int l = 0; l < Length; l++)
        {
            for (int w = 0; w < Width; w++)
            {
                text += "Column:" + l.ToString() + "," + w.ToString() + "  ";
                for (int h = 0; h < Height; h++)
                {
                    text += boardState[l, w, h].ToString() + ",";
                }
                text += "\n";
            }
            text += "\n";
        }

        return text;
    }
}
