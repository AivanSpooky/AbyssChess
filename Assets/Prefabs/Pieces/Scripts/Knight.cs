using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : BasePiece
{
    private void Awake()
    {
        Type = PieceType.KNIGHT;
        State = name.Contains("White") ? PieceState.White : PieceState.Black;
    }

    public override List<Vector2Int> GetPossibleMoves(GameObject[,] board, bool enterOutside = false)
    {
        OutsideEnter = enterOutside;
        if (State == PieceState.Deleted || State == PieceState.Empty)
            return null;

        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // Possible knight moves (L-shape)
        int[] knightMovesRow = { 2, 1, -1, -2, -2, -1, 1, 2 };
        int[] knightMovesCol = { 1, 2, 2, 1, -1, -2, -2, -1 };

        for (int i = 0; i < knightMovesRow.Length; i++)
        {
            int newRow = Row + knightMovesRow[i];
            int newCol = Column + knightMovesCol[i];

            if (IsValidMove(newCol, newRow, board))
            {
                possibleMoves.Add(new Vector2Int(newCol, newRow));
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
        return (piece == null || piece.State != PieceState.Deleted && (piece.State == PieceState.Empty || piece.State != State));
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
