using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chessboard))]
public class BoardInputHandler : MonoBehaviour, IInputHandler
{
    private Chessboard _chessboard;

    private void Awake()
    {
        _chessboard = GetComponent<Chessboard>();
    }
    public void ProcessInput(Vector3 pos, GameObject selectedObject, Action callback)
    {
        _chessboard.OnCellSelected(pos);
    }
}
