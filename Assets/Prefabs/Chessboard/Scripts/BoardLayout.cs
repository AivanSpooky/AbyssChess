using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLayout : ScriptableObject
{
    public const int TILES_PER_X = 8;
    public const int TILES_PER_Y = 8;

    public int getRows() {return TILES_PER_Y;}

    public GameObject[,] tiles;
    public GameObject[,] pieces;

    public List<GameObject> whitePieces = new List<GameObject>();
    public List<GameObject> blackPieces = new List<GameObject>();

    public GameObject[,] GetPieces()
    {
        return pieces;
    }
    public GameObject[,] GetTiles()
    {
        return tiles;
    }
    public List<GameObject> GetWhitePieces()
    {
        return whitePieces;
    }
    public List<GameObject> GetBlackPieces()
    {
        return blackPieces;
    }

    public void InitializeTiles(GameObject cellPrefab, Transform parentTransform, float scaleX, float scaleZ)
    {
        tiles = new GameObject[TILES_PER_X, TILES_PER_Y];
        for (int i = 0; i < TILES_PER_X; i++)
        {
            for (int j = 0; j < TILES_PER_Y; j++)
            {
                tiles[i, j] = CreateOneTile(cellPrefab, parentTransform, i, j, scaleX, scaleZ);
            }
        }
    }

    public void InitializePieces()
    {
        pieces = new GameObject[TILES_PER_X, TILES_PER_Y];
    }

    private GameObject CreateOneTile(GameObject cellPrefab, Transform parentTransform, int i, int j, float scaleX, float scaleZ)
    {
        GameObject tile = GameObject.Instantiate(cellPrefab, new Vector3(i, 0, j), Quaternion.identity, parentTransform);
        tile.name = $"Tile_{i}_{j}";
        tile.layer = LayerMask.NameToLayer("Tiles");
        tile.transform.localScale = new Vector3(scaleX, tile.transform.localScale.y, scaleZ);

        bool isWhite = (i + j) % 2 != 0;
        cell cellComponent = tile.GetComponent<cell>();
        cellComponent.ChangeCoords(i, j);
        cellComponent.ChangeColor(isWhite);

        return tile;
    }

    public void PlacePiece(GameObject piecePrefab, int x, int y, BasePiece.PieceState pieceState, Transform parentTransform, float pieceScaleFactor, Chessboard chessboard)
    {
        if (pieces[x, y] != null)
        {
            throw new InvalidOperationException($"A piece already exists at position ({x}, {y})");
        }

        GameObject piece = GameObject.Instantiate(piecePrefab, new Vector3(x, 0, y), Quaternion.identity, parentTransform);

        // Математические преобразования над существующей фигурой
        if (pieceState != BasePiece.PieceState.Deleted)
        {
            Vector3 newScale = CalculatePieceScale(piece, pieceScaleFactor);
            piece.transform.localScale = newScale;

            float yOffset = CalculatePieceYOffset(piece, parentTransform);
            piece.transform.position += new Vector3(0, yOffset, 0);

            piece.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        piece.GetComponent<BasePiece>().Initialize(pieceState, x, y, chessboard);

        pieces[x, y] = piece;
        if (pieceState == BasePiece.PieceState.White)
            whitePieces.Add(piece);
        else if (pieceState == BasePiece.PieceState.Black)
            blackPieces.Add(piece);
    }

    private Vector3 CalculatePieceScale(GameObject piece, float pieceScaleFactor)
    {
        Vector3 cellSize = piece.GetComponentInParent<Chessboard>().CellPrefab.GetComponent<Renderer>().bounds.size;
        Vector3 pieceSize = piece.GetComponent<Renderer>().bounds.size;

        float scaleToFit = Mathf.Min(cellSize.x / pieceSize.x, cellSize.z / pieceSize.z);
        Vector3 pieceScale = piece.transform.localScale * scaleToFit;

        float scaleFactor = 0.9f;
        pieceScale *= scaleFactor;

        if (piece.GetComponent<BasePiece>().Type == BasePiece.PieceType.KING || piece.GetComponent<BasePiece>().Type == BasePiece.PieceType.QUEEN)
            pieceScale *= 1.4f;
        else if (piece.GetComponent<BasePiece>().Type == BasePiece.PieceType.PAWN)
            pieceScale *= 0.8f;

        return new Vector3(pieceScale.x * pieceScaleFactor, pieceScale.y * pieceScaleFactor, pieceScale.z * pieceScaleFactor);
    }

    private float CalculatePieceYOffset(GameObject piece, Transform parentTransform)
    {
        Vector3 cellSize = piece.GetComponentInParent<Chessboard>().CellPrefab.GetComponent<Renderer>().bounds.size;
        float cellOffset = 0.5f;

        if (piece.GetComponent<BasePiece>().Type == BasePiece.PieceType.PAWN)
        {
            cellOffset = 0.85f;
        }

        return cellSize.y * cellOffset;
    }

    public void RemoveWhitePiece(GameObject piece)
    {
        whitePieces.Remove(piece);
    }
    public void RemoveBlackPiece(GameObject piece)
    {
        blackPieces.Remove(piece);
    }
}
