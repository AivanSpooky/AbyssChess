using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletedPiece : BasePiece
{
    public override List<Vector2Int> GetPossibleMoves(GameObject[,] board, bool enterOutside = false)
    {
        throw new System.NotImplementedException();
    }

    protected override bool IsValidMove(int col, int row, GameObject[,] board)
    {
        throw new System.NotImplementedException();
    }

    protected override bool IsValidCapture(int col, int row, GameObject[,] board)
    {
        throw new System.NotImplementedException();
    }

    protected override bool IsValidIfChecked(int col, int row, GameObject[,] board)
    {
        throw new System.NotImplementedException();
    }
}
