using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static BasePiece;

public class Chessboard : MonoBehaviour
{
    public float SCALE_FACTOR_X = 1.0f;
    public float SCALE_FACTOR_Z = 1.0f;
    public float PIECE_SCALE_FACTOR = 0.3f;
    public float cameraChangeTime = 0.7f;
    public float rotationCoeff = 1.2f;

    public GameObject gameManager;

    public GameObject CellPrefab;

    public GameObject ChessBishopBlack;
    public GameObject ChessBishopWhite;
    public GameObject ChessKingBlack;
    public GameObject ChessKingWhite;
    public GameObject ChessKnightBlack;
    public GameObject ChessKnightWhite;
    public GameObject ChessPawnBlack;
    public GameObject ChessPawnWhite;
    public GameObject ChessQueenBlack;
    public GameObject ChessQueenWhite;
    public GameObject ChessRookBlack;
    public GameObject ChessRookWhite;

    public Camera curCamera;
    public Camera whiteCamera;
    public Camera blackCamera;
    private BasePiece selectedPiece = null;
    private List<Vector2Int> highlightedCells = new List<Vector2Int>();
    public BoardLayout boardLayout;

    public Camera inputCamera;

    public GameObject TextPrefab;

    // ДЕЙСТВИЯ, ВЫПОЛНЯЕМЫЕ ПРИ ЗАПУСКЕ СЦЕНЫ
    private void Awake()
    {
        boardLayout = BoardLayout.CreateInstance<BoardLayout>();
        boardLayout.InitializeTiles(CellPrefab, transform, SCALE_FACTOR_X, SCALE_FACTOR_Z);
        boardLayout.InitializePieces();
        GeneratePieces();
        GenerateMarkings();
    }

    private void GenerateMarkings()
    {
        float offset = 0.5f;
        // Буквы от A до H
        char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        // Цифры от 1 до 8
        char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8' };

        // Генерация букв на горизонтальных координатах
        for (int i = 0; i < letters.Length; i++)
        {
            // Нижняя сторона (координаты от (0, -1) до (7, -1))
            CreateText(letters[i].ToString(), new Vector3(i, 0.51f, -2 - offset/4));

            // Верхняя сторона (координаты от (7, 8) до (0, 8))
            CreateText(letters[i].ToString(), new Vector3(/*(letters.Length - 1)-i*/i, 0.51f, 7 + 2), true);
        }

        // Генерация цифр на вертикальных координатах
        for (int i = 0; i < numbers.Length; i++)
        {
            // Левая сторона (координаты от (-1, 0) до (-1, 7))
            CreateText(numbers[i].ToString(), new Vector3(-1, 0.51f, i - offset*2));

            // Правая сторона (координаты от (8, 7) до (8, 0))
            CreateText(numbers[i].ToString(), new Vector3(8, 0.51f, i - offset * 2+2/*(numbers.Length - 1) - i - offset * 2 + 2*/), true);
        }
    }

    private void CreateText(string text, Vector3 position, bool needToRotate=false)
    {
        float zRot = 0;
        if (needToRotate)
            zRot = -180;
        GameObject textObject = Instantiate(TextPrefab, position, Quaternion.Euler(90, 0, zRot));
        textObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = text;
        }
        else
        {
            Debug.LogError("TextPrefab does not have a TextMesh component.");
        }
    }

    // ДЕЙСТВИЯ, ВЫПОЛНЯЕМЫЕ ЕЖЕКАДРОВО
    private void Update()
    {
        /*curCamera = Camera.current;*/
    }

    /*public void SetCamera()
    {
        TurnManager.TurnEnum turn = gameManager.GetComponent<TurnManager>().curTurn;
        switch (turn)
        {
            case TurnManager.TurnEnum.White:
                {
                    if (curCamera != whiteCamera)
                    {
                        Debug.Log("Switching to white camera");
                        curCamera = whiteCamera;
                        whiteCamera.enabled = true;
                        whiteCamera.GetComponent<AudioListener>().enabled = true;
                        blackCamera.enabled = false;
                        blackCamera.GetComponent<AudioListener>().enabled = false;
                    }
                    break;
                }
            case TurnManager.TurnEnum.Black:
                {
                    if (curCamera != blackCamera)
                    {
                        Debug.Log("Switching to black camera");
                        curCamera = blackCamera;
                        blackCamera.enabled = true;
                        blackCamera.GetComponent<AudioListener>().enabled = true;
                        whiteCamera.enabled = false;
                        whiteCamera.GetComponent<AudioListener>().enabled = false;
                    }
                    break;
                }
        }
        this.GetComponent<ColliderInputReciever>().UpdateCamera();
    }*/
    public void SetCamera()
    {
        TurnManager.TurnEnum turn = gameManager.GetComponent<TurnManager>().curTurn;
        switch (turn)
        {
            case TurnManager.TurnEnum.Black:
                {
                    if (curCamera.transform.position != whiteCamera.transform.position)
                    {
                        Debug.Log("Switching to white camera");
                        StartCoroutine(SmoothCameraTransition(blackCamera, whiteCamera, cameraChangeTime));
                    }
                    break;
                }
            case TurnManager.TurnEnum.White:
                {
                    if (curCamera.transform.position != blackCamera.transform.position)
                    {
                        Debug.Log("Switching to black camera");
                        StartCoroutine(SmoothCameraTransition(whiteCamera, blackCamera, cameraChangeTime));
                    }
                    break;
                }
        }
        this.GetComponent<ColliderInputReciever>().UpdateCamera();
    }

    private IEnumerator SmoothCameraTransition(Camera fromCamera, Camera toCamera, float duration)
    {
        // Сохранение начальных и конечных позиций и поворотов
        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;
        Vector3 endPosition = toCamera.transform.position;
        Quaternion endRotation = toCamera.transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Линейная интерполяция позиции с коэффициентом 0.5 для ускорения
            curCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            // Линейная интерполяция поворота с коэффициентом 1 для нормальной скорости
            curCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t * rotationCoeff);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедиться, что камера в конечной позиции и повороте
        curCamera.transform.position = endPosition;
        curCamera.transform.rotation = endRotation;

        fromCamera.enabled = false;
        fromCamera.GetComponent<AudioListener>().enabled = false;
    }

    // ВЫБРАТЬ НОВУЮ ФИГУРУ ДЛЯ ХОДА (подразумевается выполнение следующих действий)
    // 1) убрать из выбора(фокуса) старую фигуру
    // 2) выбрать новую фигуру (фокус на новой фигуре)
    // 3) подсветить все клетки, куда может сходить новая выбранная фигура
    private void SelectPiece(BasePiece piece)
    {
        // 1)
        DeselectPiece();
        // 2)
        selectedPiece = piece;
        // 3)
        List<Vector2Int> possibleMoves = piece.GetPossibleMoves(boardLayout.pieces);
        HighlightCells(possibleMoves);
    }

    private void DeselectPiece()
    {
        if (selectedPiece != null)
        {
            ResetCellColors(highlightedCells);
            selectedPiece = null;
            highlightedCells.Clear();
        }
    }

    private void HighlightCells(List<Vector2Int> cells)
    {
        BasePiece.PieceState state;
        Color fillCol = Color.green;
        cell.CellStates cellState = cell.CellStates.Default;
        foreach (Vector2Int cellPos in cells)
        {
            GameObject tile = boardLayout.tiles[cellPos.x, cellPos.y];
            if (tile != null)
            {
                cell cellComponent = tile.GetComponent<cell>();
                if (boardLayout.pieces[cellPos.x, cellPos.y] != null)
                    state = boardLayout.pieces[cellPos.x, cellPos.y].GetComponent<BasePiece>().State;
                else
                    state = BasePiece.PieceState.Empty;
                // СЪЕДАНИЕ ФИГУРЫ => КРАСНЫЙ ЦВЕТ
                if (state == BasePiece.PieceState.White || state == BasePiece.PieceState.Black)
                {
                    fillCol = Color.red;
                    cellState = cell.CellStates.Capture;
                }
                else
                {
                    fillCol = Color.green;
                    cellState = cell.CellStates.Free;
                }

                cellComponent.ChangeColor(fillCol, cellState);
                highlightedCells.Add(cellPos);
            }
        }
    }
    // !!!!
    private void ResetCellColors(List<Vector2Int> cells)
    {
        foreach (Vector2Int cellPos in cells)
        {
            GameObject tile = boardLayout.tiles[cellPos.x, cellPos.y];
            if (tile != null)
            {
                cell cellComponent = tile.GetComponent<cell>();
                cellComponent.ResetColor();
                /*bool isWhite = (cellPos.x + cellPos.y) % 2 != 0;
                cellComponent.ChangeColor(isWhite);*/
            }
        }
    }

    // ФУНКЦИЯ ИЗНАЧАЛЬНОЙ ГЕНЕРАЦИИ ФИГУР
    private void GeneratePieces()
    {
        // White pieces //
        boardLayout.PlacePiece(ChessRookWhite, 0, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessRookWhite, 7, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessKnightWhite, 1, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessKnightWhite, 6, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessBishopWhite, 2, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessBishopWhite, 5, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessQueenWhite, 3, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessKingWhite, 4, 0, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        for (int i = 0; i < BoardLayout.TILES_PER_X; i++)
        { //1
            boardLayout.PlacePiece(ChessPawnWhite, i, 1, BasePiece.PieceState.White, transform, PIECE_SCALE_FACTOR, this);
        }

        // Black pieces //
        boardLayout.PlacePiece(ChessRookBlack, 0, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessRookBlack, 7, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessKnightBlack, 1, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessKnightBlack, 6, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessBishopBlack, 2, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessBishopBlack, 5, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessQueenBlack, 3, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        boardLayout.PlacePiece(ChessKingBlack, 4, 7, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        for (int i = 0; i < BoardLayout.TILES_PER_X; i++)
        { //6
            boardLayout.PlacePiece(ChessPawnBlack, i, 6, BasePiece.PieceState.Black, transform, PIECE_SCALE_FACTOR, this);
        }
    }

    protected bool wrongCoords(int col, int row)
    {
        if (row < 0 || row >= boardLayout.getRows()  || col < 0 || col >= boardLayout.getRows())
            return true;
        return false;
    }
    // ГЛАВНЫЙ МЕТОД ОБРАБОТКИ КЛЕТОК!!!
    public void OnCellSelected(Vector3 pos)
    {
        Vector2Int coords = CalculateCoordsFromPos(pos);
        if (wrongCoords(coords[0], coords[1]))
            return;
        /*Debug.Log($"3D: {pos}, 2D: {coords}");*/
        BasePiece curpiece = ChoosePieceByCoords(coords);
        if (selectedPiece != null)
            DelightCellByCoords(new Vector2Int(selectedPiece.Column, selectedPiece.Row));
        if (boardLayout.tiles[coords.x, coords.y] == null)
            return;
        cell clickedCell = boardLayout.tiles[coords.x, coords.y].GetComponent<cell>();

        // ВЫБРАЛИ ФИГУРУ И КЛИКНУЛИ НА ОДНУ ИЗ ПОДСВЕЧЕНЫХ КЛЕТОК
        if (curpiece == null && clickedCell.State != cell.CellStates.Default // ФИГУРА - СВОБОДНАЯ КЛЕТКА
            || (curpiece != null && selectedPiece != null && Math.Abs(curpiece.State - selectedPiece.State) == 1) && clickedCell.State == cell.CellStates.Capture) // ФИГУРА ОДНОГО ЦВЕТА - ФИГУРА ПРОТИВОПОЛОЖНОГО ЦВЕТА
        {
            if (clickedCell.State != cell.CellStates.Selected)
            {
                /*Debug.Log($"EXECUTE TURN");*/
                Vector2Int src = new Vector2Int(selectedPiece.Column, selectedPiece.Row);
                gameManager.GetComponent<TurnManager>().ExecuteTurn(src, coords);
            }
            DeselectPiece();
            if (selectedPiece != null)
                DelightCellByCoords(new Vector2Int(selectedPiece.Column, selectedPiece.Row));
        }

        // НЕПРАВИЛЬНО ВЫБРАЛИ КЛЕТКУ

        else if ((curpiece == null && clickedCell.State == cell.CellStates.Default) // ФИГУРА - ПУСТАЯ КЛЕТКА
            || curpiece.State == BasePiece.PieceState.Empty || curpiece.State == BasePiece.PieceState.Deleted // КЛИКНУЛИ НА УДАЛЕННУЮ ИЛИ ПУСТУЮ КЛЕТКУ ИЗНАЧАЛЬНО 
            || (curpiece != null && curpiece == selectedPiece)) // КЛИКНУЛИ НА ВЫБРАННУЮ ФИГУРУ ДВАЖДЫ
        {
            /*Debug.Log($"WRONG CELL");*/
            DeselectPiece();
        }

        // ВЫБРАЛИ ФИГУРУ

        else if (curpiece != null && curpiece != selectedPiece)
        {
            // Если цвет хода совпадает с цветом фигуры:
            if ((int)gameManager.GetComponent<TurnManager>().GetCurrentTurn() == (int)curpiece.State)
            {
                /*Debug.Log($"NEW FIG");*/
                SelectPiece(curpiece);
                HighlightCellByCoords(coords);
            }
            else
            {
                /*Debug.Log($"SAME FIG");*/
                DeselectPiece();
            }
        }
    }

    // ПОЛУЧИТЬ ФИГУРУ ИЗ BOARDLAYOUT ПО ШАХМАТНЫМ КООРДИНАТАМ
    private BasePiece ChoosePieceByCoords(Vector2Int coords)
    {
        if (ValidCoords(coords) && boardLayout.pieces[coords.x, coords.y] != null)
            return boardLayout.pieces[coords.x, coords.y].GetComponent<BasePiece>();
        return null;
    }

    // ДИЗАЙН КЛЕТКИ С ВЫБРАННОЙ ФИГУРОЙ
    private void HighlightCellByCoords(Vector2Int coords)
    {
        if (ValidCoords(coords))
            boardLayout.tiles[coords.x, coords.y].GetComponent<cell>().ChangeColor(Color.yellow, cell.CellStates.Selected);
    }
    // ДИЗАЙН КЛЕТКИ (ОСТАЛСЯ ОДИН РАУНД ДО УДАЛЕНИЯ)
    public void HighlightCellByCoordsLastRound(Vector2Int coords)
    {
        if (ValidCoords(coords) && boardLayout.tiles[coords.x, coords.y] != null)
            boardLayout.tiles[coords.x, coords.y].GetComponent<cell>().ChangeOriginalColor(Color.cyan);
    }

    // ВОЗВРАЩАЕМ КЛЕТКУ К ИЗНАЧАЛЬНОЙ ЧЕРНО-БЕЛОЙ РАСЦВЕТКЕ
    private void DelightCellByCoords(Vector2Int coords)
    {
        if (ValidCoords(coords))
            boardLayout.tiles[coords.x, coords.y].GetComponent<cell>().ResetColor();
    }

    // ПРАВИЛЬНО ЛИ ЗАДАНЫ КООРДИНАТЫ
    private bool ValidCoords(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= BoardLayout.TILES_PER_X || coords.y < 0 || coords.y >= BoardLayout.TILES_PER_Y)
            return false;
        return true;
    }

    // ПЕРЕВОД ЭКРАННЫХ КООРДИНАТ В ШАХМАТНЫЕ (МАТРИЦА 8х8)
    private Vector2Int CalculateCoordsFromPos(Vector3 pos)
    {
        Vector3 cellSize = CellPrefab.GetComponent<Renderer>().bounds.size;
        // Adjust these values based on your actual board setup
        float cellSizeX = cellSize.x;
        float cellSizeZ = cellSize.z;

        int x = Mathf.FloorToInt((pos.x + cellSize.x / 2) / cellSizeX);
        int y = Mathf.FloorToInt((pos.z + cellSize.z / 2) / cellSizeZ);

        return new Vector2Int(x, y);
    }

    // ПОЛУЧИТЬ СПИСОК ФИГУР ИЗ BOARDLAYOUT
    public GameObject[,] GetPieces()
    {
        return boardLayout.GetPieces();
    }
    public GameObject[,] GetTiles()
    {
        return boardLayout.GetTiles();
    }
    public List<GameObject> GetWhitePieces()
    {
        return boardLayout.GetWhitePieces();
    }
    public List<GameObject> GetWhitePieces(GameObject[,] pieces)
    {
        List<GameObject> whitePieces = new List<GameObject>();

        // Проход по всему массиву pieces
        for (int x = 0; x < pieces.GetLength(0); x++)
        {
            for (int y = 0; y < pieces.GetLength(1); y++)
            {
                GameObject piece = pieces[x, y];
                if (piece != null)
                {
                    BasePiece basePiece = piece.GetComponent<BasePiece>();
                    if (basePiece != null && basePiece.State == PieceState.White)
                    {
                        whitePieces.Add(piece);
                    }
                }
            }
        }
        return whitePieces;
    }
    public Vector2Int GetWhiteKingCoords()
    {
        List<GameObject> whites = boardLayout.GetWhitePieces();
        Vector2Int coords = new Vector2Int(-1, -1);
        foreach (var pieceObj in whites)
        {
            BasePiece piece = pieceObj.GetComponent<BasePiece>();
            if (piece.Type == BasePiece.PieceType.KING)
            {
                coords.x = piece.Column;
                coords.y = piece.Row;
            }
        }
        return coords;
    }
    public Vector2Int GetWhiteKingCoords(GameObject[,] pieces)
    {
        List<GameObject> whites = GetWhitePieces(pieces);
        Vector2Int coords = new Vector2Int(-1, -1);
        foreach (var pieceObj in whites)
        {
            BasePiece piece = pieceObj.GetComponent<BasePiece>();
            if (piece.Type == BasePiece.PieceType.KING)
            {
                coords.x = piece.Column;
                coords.y = piece.Row;
            }
        }
        return coords;
    }
    public List<GameObject> GetBlackPieces()
    {
        return boardLayout.GetBlackPieces();
    }
    public List<GameObject> GetBlackPieces(GameObject[,] pieces)
    {
        List<GameObject> blackPieces = new List<GameObject>();

        // Проход по всему массиву pieces
        for (int x = 0; x < pieces.GetLength(0); x++)
        {
            for (int y = 0; y < pieces.GetLength(1); y++)
            {
                GameObject piece = pieces[x, y];
                if (piece != null)
                {
                    BasePiece basePiece = piece.GetComponent<BasePiece>();
                    if (basePiece != null && basePiece.State == PieceState.Black)
                    {
                        blackPieces.Add(piece);
                    }
                }
            }
        }
        return blackPieces;
    }
    public Vector2Int GetBlackKingCoords()
    {
        List<GameObject> whites = boardLayout.GetBlackPieces();
        Vector2Int coords = new Vector2Int(-1, -1);
        foreach (var pieceObj in whites)
        {
            BasePiece piece = pieceObj.GetComponent<BasePiece>();
            if (piece.Type == BasePiece.PieceType.KING)
            {
                coords.x = piece.Column;
                coords.y = piece.Row;
            }
        }
        return coords;
    }

    public Vector2Int GetGradedWhitePawn()
    {
        List<GameObject> whites = boardLayout.GetWhitePieces();
        Vector2Int coords = new Vector2Int(-1, -1);
        foreach (var pieceObj in whites)
        {
            BasePiece piece = pieceObj.GetComponent<BasePiece>();
            if (piece.Type == BasePiece.PieceType.PAWN && piece.State == PieceState.White && piece.Row == boardLayout.getRows() - 1)
            {
                coords.x = piece.Column;
                coords.y = piece.Row;
                break;
            }
        }
        return coords;
    }

    public Vector2Int GetGradedBlackPawn()
    {
        List<GameObject> whites = boardLayout.GetBlackPieces();
        Vector2Int coords = new Vector2Int(-1, -1);
        foreach (var pieceObj in whites)
        {
            BasePiece piece = pieceObj.GetComponent<BasePiece>();
            if (piece.Type == BasePiece.PieceType.PAWN && piece.State == PieceState.Black && piece.Row == 0)
            {
                coords.x = piece.Column;
                coords.y = piece.Row;
                break;
            }
        }
        return coords;
    }

    public Vector2Int GetBlackKingCoords(GameObject[,] pieces)
    {
        List<GameObject> whites = GetBlackPieces(pieces);
        Vector2Int coords = new Vector2Int(-1, -1);
        foreach (var pieceObj in whites)
        {
            BasePiece piece = pieceObj.GetComponent<BasePiece>();
            if (piece.Type == BasePiece.PieceType.KING)
            {
                coords.x = piece.Column;
                coords.y = piece.Row;
            }
        }
        return coords;
    }
    public void RemoveWhitePiece(GameObject piece)
    {
        boardLayout.RemoveWhitePiece(piece);
    }
    public void RemoveBlackPiece(GameObject piece)
    {
        boardLayout.RemoveBlackPiece(piece);
    }
    public GameObject[,] CloneBoard(GameObject[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        GameObject[,] newBoard = new GameObject[rows, cols];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (original[x, y] != null)
                {
                    newBoard[x, y] = Instantiate(original[x, y]);
                    BasePiece old = original[x, y].GetComponent<BasePiece>();
                    newBoard[x, y].GetComponent<BasePiece>().Initialize(old.State, old.Column, old.Row, this);
                }
                else
                    newBoard[x, y] = null;
            }
        }

        return newBoard;
    }
}
