using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] Renderer pieceRenderer;
    [SerializeField] Rigidbody pieceRigidbody;

    [SerializeField] Material highlight;
    [SerializeField] Material hide;

    public static event Action<bool> PieceInPlay;

    private void Start()
    {
        PieceInPlay?.Invoke(true);
        Invoke("Freeze", 1.5f);
    }

    private void Freeze()
    {
        pieceRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        pieceRigidbody.velocity = Vector3.zero;

        PieceInPlay?.Invoke(false);
    }

    public void Highlight()
    {
        pieceRenderer.material = highlight;
    }
    public void Hide()
    {
        pieceRenderer.material = hide;
    }
}
