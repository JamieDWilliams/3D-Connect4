using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Column : MonoBehaviour
{
    private int l, w;
    [SerializeField] Transform SpawnLocation;

    [SerializeField] Renderer columnRenderer;
    [SerializeField] Material normal;
    [SerializeField] Material highlight;
   
    public static event Action<int, int, Transform> Selected;
    public static event Action<int, int, Transform> Hover;
    public static event Action Exit;

    void Start()
    {
        l = int.Parse(transform.parent.parent.name);
        w = int.Parse(transform.parent.name);
        
        //Debug.Log(transform.parent.parent.name +" "+ transform.parent.name);
    }

    private void OnMouseDown()
    {
        Debug.Log(l + " " + w);
        if (GameManager.Instance.ValidMove(l, w))
        {
            Selected(l, w, SpawnLocation);
        }
        columnRenderer.material = normal;
        Exit();
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.ValidMove(l, w))
        {
            Hover(l, w, SpawnLocation);
            columnRenderer.material = highlight;
        }
        else
        {
            Exit();
        }
    }

    private void OnMouseExit() {
        columnRenderer.material = normal;
        Exit();
    }

}
