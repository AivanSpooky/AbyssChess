using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : BasePiece
{
    private void Awake()
    {
        Type = PieceType.KING;
        State = name.Contains("White") ? PieceState.White : PieceState.Black;
    }
    public override List<Vector2Int> GetPossibleMoves(GameObject[,] board, bool enterOutside = false)
    {
        OutsideEnter = enterOutside;
        if (State == PieceState.Deleted || State == PieceState.Empty)
            return null;

        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // MOVE RULE
        int[] dRow = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dCol = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < dRow.Length; i++)
        {
            int newRow = Row + dRow[i];
            int newCol = Column + dCol[i];

            if (IsValidMove(newCol, newRow, board))
            {
                possibleMoves.Add(new Vector2Int(newCol, newRow));
            }
        }

        return possibleMoves;
    }

    protected override bool IsValidMove(int col, int row, GameObject[,] board)
    {
        if (row < 0 || row >= board.GetLength(0) || col < 0 || col >= board.GetLength(1) || !IsValidIfChecked(col, row, board)/* || wrongKingMove(col, row)*/)
        {
            return false;
        }

        if (board[col, row] == null) { return true; }

        BasePiece piece = board[col, row].GetComponent<BasePiece>();
        return (piece == null || (piece.State != PieceState.Deleted && piece.State != State));
    }

    private bool wrongKingMove(int col, int row)
    {
        Vector2Int futurePos = new Vector2Int(col, row);
        switch (State)
        {
            case PieceState.White:
                {
                    List<GameObject> blacks = Chessboard.GetBlackPieces();
                    foreach (GameObject black in blacks)
                    {
                        BasePiece bp = black.GetComponent<BasePiece>();
                        List<Vector2Int> possibleMoves = bp.GetPossibleMoves(Chessboard.GetPieces());
                        foreach (var possibleMove in possibleMoves)
                            if (possibleMove == futurePos)
                            {
                                blacks.Clear();
                                return true;
                            }
                    }
                    blacks.Clear();
                    break;
                }
            case PieceState.Black:
                {
                    List<GameObject> whites = Chessboard.GetWhitePieces();
                    foreach (GameObject white in whites)
                    {
                        BasePiece bp = white.GetComponent<BasePiece>();
                        List<Vector2Int> possibleMoves = bp.GetPossibleMoves(Chessboard.GetPieces());
                        foreach (var possibleMove in possibleMoves)
                            if (possibleMove == futurePos)
                            {
                                whites.Clear();
                                return true;
                            }
                    }
                    whites.Clear();
                    break;
                }
        }
        return false;
    }

    protected override bool IsValidCapture(int col, int row, GameObject[,] board)
    {
        throw new System.NotImplementedException();
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
