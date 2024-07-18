using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePiece : MonoBehaviour
{
    // STATES OF A PIECE ON CURRENT CELL //
    //
    // EMPTY - NO PIECE ON THE CELL
    // WHITE - WHITE PIECE ON THE CELL
    // BLACK - BLACK PIECE ON THE CELL
    // DELETED - THE CELL DOES NOT EXIST

    protected Chessboard Chessboard;
    public enum PieceState
    {
        Empty,
        White = 101,
        Black = 102,
        Deleted = 200
    }
    public enum PieceType
    {
        NONE,
        PAWN,
        KNIGHT,
        BISHOP,
        ROOK,
        QUEEN,
        KING
    }
    // PIECE STATE
    public PieceState State { get; set; }
    // PIECE TYPE
    public PieceType Type { get; set; }
    // PIECE ROW
    public int Row { get; set; }
    // PIECE COLUMN
    public int Column { get; set; }
    // MADE A TURN
    public bool Moved = false;
    public bool OutsideEnter = false;
    // INIT
    public void Initialize(PieceState state, int column, int row, Chessboard chessboard)
    {
        State = state;
        Row = row;
        Column = column;
        Chessboard = chessboard;
    }

    // Abstract method for getting possible piece moves
    public abstract List<Vector2Int> GetPossibleMoves(GameObject[,] board, bool enterOutside=false);
    protected abstract bool IsValidMove(int col, int row, GameObject[,] board);
    protected abstract bool IsValidCapture(int col, int row, GameObject[,] board);
    protected abstract bool IsValidIfChecked(int col, int row, GameObject[,] board);
}
