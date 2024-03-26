using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    public GameManager gm;
    private int[] column = new int[2];

    void Start()
    {
        column[0] = int.Parse(transform.parent.parent.name);
        column[1] = int.Parse(transform.parent.name);
        //Debug.Log(transform.parent.parent.name +" "+ transform.parent.name);
    }

    private void OnMouseDown()
    {
        gm.SelectColumn(column);
        Debug.Log(column[0] + " " + column[1]);
    }

    private void OnMouseOver()
    {
        gm.HoverOverColumn(column);
    }

    private void OnMouseExit() {
        gm.ExitColumn();    
    }
}
