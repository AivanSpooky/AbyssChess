using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInputReciever : InputReciever
{
    private Vector3 clickPos;
    private Camera boardCamera;
    private Chessboard chessboard;

    private void Start()
    {
        chessboard = FindObjectOfType<Chessboard>();
        if (chessboard != null)
        {
            boardCamera = chessboard.curCamera;
        }
        else
        {
            Debug.LogError("Chessboard not found in the scene. Make sure you have a Chessboard GameObject.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = boardCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                clickPos = hit.point;
                OnInputRecieved();
            }
        }
    }

    public void UpdateCamera()
    {
        if (chessboard != null)
        {
            boardCamera = chessboard.curCamera;
        }
    }

    public override void OnInputRecieved()
    {
        foreach (var handler in inputHandlers)
        {
            handler.ProcessInput(clickPos, null, null);
        }
    }
}