using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject player1;
    public GameObject player2;

    public GameObject player1Ghost;
    public GameObject player2Ghost;

    public GameMenu gameMenu;

    public int length = 4, width = 4, height = 4;
    public int winLength;

    public GameObject columns;
    private GameObject[,] spawnLocs = new GameObject[4,4];

    GameObject fallingPiece;

    private bool player1Turn = true;

    private GameObject[,,] boardState;

    void Start()
    {
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        
        //Populate spawnLocs with the predefined spawning locations
        for (int i = 0; i < columns.transform.childCount; i++)
        {
            Transform row = columns.transform.GetChild(i);
            for (int j = 0; j < row.childCount; j++)
            {
                Transform cylinder = row.GetChild(j);

                spawnLocs[i, j] = cylinder.Find("SpawnLoc").gameObject;              
            }
        }

        boardState = new GameObject[length,width,height];
    }

    public void SelectColumn(int[] column)
    {
        int highestPiece = HighestPieceInColumn(column);

        if (LegalMove(column, highestPiece) && FallingPieceStationary()) {
            TakeTurn(column, highestPiece);
        }
    }

    public void HoverOverColumn(int[] column)
    {
        if (HighestPieceInColumn(column) < height - 1 && FallingPieceStationary())
        {
            if (player1Turn)
            {
                player1Ghost.SetActive(true);
                player1Ghost.transform.position = spawnLocs[column[0], column[1]].transform.position;
            }

            else
            {
                player2Ghost.SetActive(true);
                player2Ghost.transform.position = spawnLocs[column[0], column[1]].transform.position;
            }
        }
    }

    public void ExitColumn()
    {
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
    }

    private void TakeTurn(int[] column, int highestPiece)
    {
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        if (player1Turn)
        {
            fallingPiece = Instantiate(player1, spawnLocs[column[0], column[1]].transform.position, Quaternion.Euler(90f, 0f, (float)UnityEngine.Random.Range(0, 360)));
            fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0.1f, 0f);
            fallingPiece.GetComponent<PieceManager>().SetPlayer(1);
            fallingPiece.GetComponent<PieceManager>().SetPosition(column[0], column[1], highestPiece + 1);
            boardState[column[0], column[1], highestPiece + 1] = fallingPiece;
            if (DidWin(fallingPiece, 1))
            {
                gameMenu.Winner(1);
            }
        }
        else
        {
            fallingPiece = Instantiate(player2, spawnLocs[column[0], column[1]].transform.position, Quaternion.Euler(90f, 0f, (float)UnityEngine.Random.Range(0, 360)));
            fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0f,0.1f,0f);
            fallingPiece.GetComponent<PieceManager>().SetPlayer(2);
            fallingPiece.GetComponent<PieceManager>().SetPosition(column[0], column[1], highestPiece + 1);
            boardState[column[0], column[1], highestPiece + 1] = fallingPiece;
            if (DidWin(fallingPiece, 2))
            {
                gameMenu.Winner(2);
            }
        }

        

        player1Turn = !player1Turn;
    }

    private bool LegalMove(int[] column, int highestPiece)
    {
        if (highestPiece < height-1)
        {
            return true;
        }
        else{
            return false;
        }
    }

    private bool FallingPieceStationary()
    {
        if (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int HighestPieceInColumn(int[] column)
    {
        int highestPiece;
        for (highestPiece = 0; highestPiece < height; highestPiece++)
        {
            if (boardState[column[0], column[1], highestPiece] == null)
            {
                highestPiece--;
                return highestPiece;
            }
        }
        return highestPiece;
    }

    private bool DidWin(GameObject piece, int playerNum)//Works for all size grids, and num of consecutive pieces required to win
    {
        int[] position = piece.GetComponent<PieceManager>().position; //0=l, 1=w, 2=h

        int bound = winLength - 1;
        GameObject[] possibleConnections = new GameObject[2 * winLength - 1];
        
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
            GameObject[] win;
            win = checkLine(GenerateLine(piece, possibleLines[line,0], possibleLines[line,1], possibleLines[line, 2]), playerNum);
            if (win != null)
            {
                for (int i = 0; i < boardState.GetLength(0); i++)
                {
                    for(int j = 0; j < boardState.GetLength(1); j++)
                    {
                        for(int k = 0; k < boardState.GetLength(2); k++)
                        {
                            try
                            {
                                boardState[i, j, k].GetComponent<PieceManager>().Hide();
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                
                for (int i = 0; i < win.Length; i++)
                {
                    win[i].GetComponent<PieceManager>().Highlight();
                }

                return true;
            }
        }
        

        return false;
    }

    //l,w,h equal values 0, -1, or 1. lwh determine the direction of the line (horizontal etc)
    private GameObject[] GenerateLine(GameObject piece, int l, int w, int h)
    {
        int[] position = piece.GetComponent<PieceManager>().position; //0->l, 1->w, 2->h

        //If n connsecutive pieces wins then pieces further than n-1 away are irrelevant. Hence, our line has size 2*(n-1)+1
        int bound = winLength - 1;
        GameObject[] line = new GameObject[2 * bound + 1];

        for (int posMult = -bound; posMult <= bound; posMult++)
        {
            try
            {
                line[posMult + bound] = boardState[position[0] + l * posMult,
                                               position[1] + w * posMult,
                                               position[2] + h * posMult];
            }
            //The line extends out the playable space
            catch (IndexOutOfRangeException) 
            {
                line[posMult + bound] = null;
            }
        }
 
        return line;
    }

    private GameObject[] checkLine(GameObject[] possibleConnections, int playerNum)
    {
        int consecutive = 0;
        for (int e = 0; e < possibleConnections.Length; e++)
        {
            
            if (possibleConnections[e] == null)
            {
                consecutive = 0;
            }
            else if (possibleConnections[e].GetComponent<PieceManager>().player == playerNum)
            {
                consecutive++;
            }
            else consecutive = 0;

            if (consecutive == winLength)
            {
                GameObject[] win = new GameObject[winLength];
                for (int i = 0; i < winLength; i++)
                {
                    win[i] = possibleConnections[e - i];
                }
                return win;
            }
        }
        return null;
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
                    text += boardState[l,w,h].ToString() + ",";
                }
                text += "\n";
            }
            text += "\n";
        }

        return text;
    }
}
