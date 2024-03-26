using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public GameObject piece;

    public Material highlight;
    public Material hide;

    public int player;
    public int[] position = { -1, -1, -1 };

    private void Start()
    {
        Invoke("Freeze", 1.5f);
    }
    public void SetPosition(int l, int w, int h)
    {
        position = new int[] { l, w, h };
    }
    public void SetPlayer(int p)
    {
        player = p;
    }

    private void Freeze()
    {
        piece.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        piece.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Highlight()
    {
        piece.GetComponent<Renderer>().material = highlight;
    }
    public void Hide()
    {
        piece.GetComponent<Renderer>().material = hide;
    }
}
