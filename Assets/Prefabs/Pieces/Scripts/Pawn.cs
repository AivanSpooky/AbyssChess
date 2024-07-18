using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Pawn : BasePiece
{
    private void Awake()
    {
        Type = PieceType.PAWN;
        State = name.Contains("White") ? PieceState.White : PieceState.Black;
    }

    public override List<Vector2Int> GetPossibleMoves(GameObject[,] board, bool enterOutside = false)
    {
        OutsideEnter = enterOutside;
        if (State == PieceState.Deleted || State == PieceState.Empty)
            return null;

        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // MOVE RULES FOR PAWN
        int moveDirection = (State == PieceState.White) ? 1 : -1; // Direction: +1 for White, -1 for Black

        // Single move forward
        int newRow = Row + moveDirection;
        int newCol = Column;
        if (IsValidMove(newCol, newRow, board))
        {
            possibleMoves.Add(new Vector2Int(newCol, newRow));

            // Double move forward for initial position
            if (!Moved)
            {
                newRow += moveDirection;
                if (IsValidMove(newCol, newRow, board))
                {
                    possibleMoves.Add(new Vector2Int(newCol, newRow));
                }
            }
        }
        else if ((int)Chessboard.gameManager.GetComponent<TurnManager>().whoInCheck == (int)State)
        {
            if (!Moved)
            {
                newRow += moveDirection;
                if (IsValidMove(newCol, newRow, board))
                {
                    possibleMoves.Add(new Vector2Int(newCol, newRow));
                }
            }
        }

        // Capture diagonally
        int[] captureCols = { Column - 1, Column + 1 };
        foreach (int captureCol in captureCols)
        {
            if (IsValidCapture(captureCol, Row + moveDirection, board))
            {
                possibleMoves.Add(new Vector2Int(captureCol, Row + moveDirection));
            }
        }

        return possibleMoves;
    }

    protected override bool IsValidMove(int col, int row, GameObject[,] board)
    {
        return IsValidIfChecked(col, row, board) && (row >= 0 && row < BoardLayout.TILES_PER_Y && col >= 0 && col < BoardLayout.TILES_PER_X &&
               (board[col, row] == null || board[col, row].GetComponent<BasePiece>().State == PieceState.Empty));
    }

    protected override bool IsValidCapture(int col, int row, GameObject[,] board)
    {
        return !wrongCoords(col, row, board) && IsValidIfChecked(col, row, board) && (row >= 0 && row < BoardLayout.TILES_PER_Y && col >= 0 && col < BoardLayout.TILES_PER_X &&
               board[col, row] != null && board[col, row].GetComponent<BasePiece>().State != PieceState.Empty && board[col, row].GetComponent<BasePiece>().State != State);
    }

    protected bool wrongCoords(int col, int row, GameObject[,] board)
    {
        if (row < 0 || row >= board.GetLength(0) || col < 0 || col >= board.GetLength(1))
            return true;
        return false;
    }

    protected override bool IsValidIfChecked(int col, int row, GameObject[,] board)
    {
        // Если наш король по шахом
        TurnManager turnM = Chessboard.gameManager.GetComponent<TurnManager>();
        int checkState = (int)turnM.whoInCheck;
        if (!OutsideEnter && !wrongCoords(col, row, board))
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
