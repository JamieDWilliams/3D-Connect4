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

    private Collider[] inputFields;

    
    public static event Func<int, int, Transform, bool> Selected;
    public static event Func<int, int, Transform, bool> Hover;
    public static event Action Exit;

    private void OnEnable()
    {
        PieceManager.PieceInPlay += SetActive;
        GameMenu.BlockInput += SetActive;
    }
    void Start()
    {
        l = int.Parse(transform.parent.parent.name);
        w = int.Parse(transform.parent.name);
        
        inputFields = GetComponents<Collider>();
        //Debug.Log(transform.parent.parent.name +" "+ transform.parent.name);
    }
    private void OnDisable()
    {
        PieceManager.PieceInPlay -= SetActive;
        GameMenu.BlockInput -= SetActive;
    }

    private void OnMouseDown()
    {
        Debug.Log(l + " " + w);
        if (Selected(l, w, SpawnLocation))
        {
            columnRenderer.material = normal;
            Exit();
        }
    }

    private void OnMouseOver()
    {
        if (Hover(l, w, SpawnLocation))
        {
            columnRenderer.material = highlight;
        }
    }

    private void OnMouseExit() {
        Exit();
        columnRenderer.material = normal;
    }

    private void SetActive(bool set)
    {
        foreach(Collider IF in inputFields)
        {
            IF.enabled = !set;
        }
    }
}
