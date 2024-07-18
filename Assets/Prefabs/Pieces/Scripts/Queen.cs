using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : BasePiece
{
    private void Awake()
    {
        Type = PieceType.QUEEN;
        State = name.Contains("White") ? PieceState.White : PieceState.Black;
    }

    public override List<Vector2Int> GetPossibleMoves(GameObject[,] board, bool enterOutside = false)
    {
        OutsideEnter = enterOutside;
        if (State == PieceState.Deleted || State == PieceState.Empty)
            return null;

        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // Directions for vertical, horizontal, and diagonal movement
        int[] dRow = { 1, -1, 0, 0, 1, -1, -1, 1 };
        int[] dCol = { 0, 0, 1, -1, 1, -1, 1, -1 };

        // Check each direction
        for (int i = 0; i < dRow.Length; i++)
        {
            int newRow = Row + dRow[i];
            int newCol = Column + dCol[i];
            while (IsValidMove(newCol, newRow, board) || ((int)Chessboard.gameManager.GetComponent<TurnManager>().whoInCheck == (int)State))
            {
                if (wrongCoords(newCol, newRow, board))
                    break;
                if (IsValidMove(newCol, newRow, board))
                    possibleMoves.Add(new Vector2Int(newCol, newRow));
                if (board[newCol, newRow] != null && (board[newCol, newRow].GetComponent<BasePiece>().State != PieceState.Empty))
                {
                    break; // Stop further moves if there's a piece blocking the path
                }
                newRow += dRow[i];
                newCol += dCol[i];
            }
        }

        return possibleMoves;
    }

    protected override bool IsValidMove(int col, int row, GameObject[,] board)
    {
        if (row < 0 || row >= board.GetLength(0) || col < 0 || col >= board.GetLength(1) || !IsValidIfChecked(col, row, board))
        {
            return false;
        }

        if (board[col, row] == null) { return true; }

        BasePiece piece = board[col, row].GetComponent<BasePiece>();
        return (piece == null || piece.State != PieceState.Deleted && (piece.State == PieceState.Empty || Mathf.Abs((int)piece.State - (int)State) == 1));
    }
    protected bool wrongCoords(int col, int row, GameObject[,] board)
    {
        if (row < 0 || row >= board.GetLength(0) || col < 0 || col >= board.GetLength(1))
            return true;
        return false;
    }

    protected override bool IsValidCapture(int col, int row, GameObject[,] board)
    {
        return IsValidMove(col, row, board);
    }

    protected override bool IsValidIfChecked(int col, int row, GameObject[,] board)
    {
        // Если наш король по шахом
        TurnManager turnM = Chessboard.gameManager.GetComponent<TurnManager>();
        int checkState = (int)turnM.whoInCheck;
        if (!OutsideEnter)
        {
            GameObject[,] tmpTiles = Chessboard.CloneBoard(board);
            switch (checkState)
            {
                case (int)PieceState.White:
                    {
                        turnM.whitePlayer.ExecuteTMPAction(new Vector2Int(Column, Row), new Vector2Int(col, row), tmpTiles);
                        bool cannotPreventCheck = turnM.checkIfWhiteInCheck(tmpTiles);
                        foreach (GameObject p in tmpTiles)
                            GameObject.Destroy(p);
                        if (cannotPreventCheck)
                            return false;
                        else
                            break;
                    }
                case (int)PieceState.Black:
                    {
                        turnM.blackPlayer.ExecuteTMPAction(new Vector2Int(Column, Row), new Vector2Int(col, row), tmpTiles);
                        bool cannotPreventCheck = turnM.checkIfBlackInCheck(tmpTiles);
                        foreach (GameObject p in tmpTiles)
                            GameObject.Destroy(p);
                        if (cannotPreventCheck)
                            return false;
                        else
                            break;
                    }
                default:
                    {
                        turnM.blackPlayer.ExecuteTMPAction(new Vector2Int(Column, Row), new Vector2Int(col, row), tmpTiles);
                        bool cannotPreventCheck = false;
                        if (State == PieceState.White)
                            cannotPreventCheck = turnM.checkIfWhiteInCheck(tmpTiles);
                        else
                            cannotPreventCheck = turnM.checkIfBlackInCheck(tmpTiles);

                        foreach (GameObject p in tmpTiles)
                            GameObject.Destroy(p);
                        if (cannotPreventCheck)
                            return false;
                        else
                            break;
                    }
            }
        }
        return true;
    }
}
