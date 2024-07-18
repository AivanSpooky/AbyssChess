using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnEnum
    {
        White = 101,
        Black = 102,
        Scenario = 200,
        NoOne = 300
    }
    public int round = 0;
    public TurnEnum curTurn = TurnEnum.White;
    public TurnEnum whoInCheck = TurnEnum.Scenario;
    public bool endGame = false;
    public bool stalemateGame = false;
    public Algo algo;
    public WhitePlayer whitePlayer;
    public BlackPlayer blackPlayer;
    public Scenario scenario;
    [SerializeField] private Chessboard chessboard;

    // Поля для звуков
    public AudioClip checkSound;
    public AudioClip checkmateSound;
    public AudioClip drawSound;
    public AudioSource audioSource;

    private void Awake()
    {
        whitePlayer.SetChessboard(chessboard);
        blackPlayer.SetChessboard(chessboard);
        scenario.SetChessboard(chessboard);
        scenario.setAnimation(GameManager.level - 1);
        FindObjectOfType<GameManager>().UpdateCurrentTurnText(curTurn);
    }

    public TurnEnum GetCurrentTurn() { return curTurn; }

    public void NextTurn()
    {
        // SOUNDS
        if (endGame)
        {
            if (stalemateGame)
                PlaySoundStalemate();
            else
                PlaySoundMate();
        }
        else if (whoInCheck == TurnEnum.White || whoInCheck == TurnEnum.Black)
        {
            PlaySoundCheck();
        }
        Debug.Log("NextTurn: " + (int)curTurn);
        if (curTurn != TurnEnum.Scenario)
            chessboard.SetCamera();
        if (endGame)
        {
            curTurn = TurnEnum.NoOne;
        }
        else
        {
            switch (curTurn)
            {
                case TurnEnum.White:
                    curTurn = TurnEnum.Black; break;
                case TurnEnum.Black:
                    curTurn = TurnEnum.Scenario; ExecuteScenarioTurn(); break;
                //curTurn = TurnEnum.White; break;
                case TurnEnum.Scenario:
                    curTurn = TurnEnum.White; break;
            }
        }
        FindObjectOfType<GameManager>().UpdateCurrentTurnText(curTurn);
    }

    public void ExecuteScenarioTurn()
    {
        //    
        ExecuteTurn(new Vector2Int(-1, -1), new Vector2Int(-1, -1));
    }

    public async void ExecuteTurn(Vector2Int oldPos, Vector2Int newPos)
    {
        switch (curTurn)
        {
            case TurnEnum.White:
                {
                    whitePlayer.ExecuteAction(oldPos, newPos);
                    break;
                }
            case TurnEnum.Black:
                {
                    blackPlayer.ExecuteAction(oldPos, newPos);
                    break;
                }
            case TurnEnum.Scenario:
                {
                    try
                    {
                        Debug.Log(algo == null);
                        List<AlgoCell> cells = algo.ExecuteRound(chessboard.GetTiles(), round);

                        StartCoroutine(ExecuteScenarioWithDelayCoroutine(cells));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Exception in TurnEnum.Scenario: " + ex.Message);
                    }
                    break;
                }
            default:
                return;
        }
        await AnalyseAfterTurn();
        FinalSolution();
    }

    private IEnumerator ExecuteScenarioWithDelayCoroutine(List<AlgoCell> cells)
    {
        yield return new WaitForSeconds(BasePlayer.animationTime); // Задержка на animationTime секунд
        scenario.ExecuteAction(cells);
        algo.Execute(chessboard.GetTiles(), round);
        algo.DeleteDeleted();
    }

    private async Task ExecuteScenarioWithDelayAsync(List<AlgoCell> cells)
    {
        /*await Task.Delay((int)(BasePlayer.animationTime*1000)+1); // Задержка на 0.5 секунд (500 миллисекунд)*/
        try
        {
            await Task.Delay((int)(BasePlayer.animationTime * 1000) + 1); // Задержка на 0.5 секунд (500 миллисекунд)
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception in ExecuteScenarioWithDelayAsync: " + ex.Message);
        }
    }

    private async Task AnalyseAfterTurn()
    {
        Vector2Int notFound = new Vector2Int(-1, -1);
        //      (     )
        Vector2Int coords = chessboard.GetGradedWhitePawn();
        if (coords != notFound)
        {
            await GradeWhitePawn(coords);
        }
        coords = chessboard.GetGradedBlackPawn();
        if (coords != notFound)
        {
            await GradeBlackPawn(coords);
        }

        //      
        if (whoInCheck != TurnEnum.Scenario)
            whoInCheck = TurnEnum.Scenario;
        //    
        switch (curTurn)
        {
            case TurnEnum.White:
                {
                    if (checkIfBlackInCheck())
                        whoInCheck = TurnEnum.Black;
                    break;
                }
            case TurnEnum.Black:
                {
                    if (checkIfWhiteInCheck())
                        whoInCheck = TurnEnum.White;
                    break;
                }
        }
    }

    bool checkIfBlackInCheck()
    {
        GameObject[,] pieces = chessboard.GetPieces();
        List<GameObject> whites = chessboard.GetWhitePieces();
        Vector2Int blackKingCoords = chessboard.GetBlackKingCoords();
        foreach (var whiteFig in whites)
        {
            BasePiece whitePiece = whiteFig.GetComponent<BasePiece>();
            List<Vector2Int> possibleMoves = whitePiece.GetPossibleMoves(pieces);
            foreach (var possibleMove in possibleMoves)
                if (possibleMove == blackKingCoords)
                    return true;
        }

        return false;
    }

    bool checkIfWhiteInMate()
    {
        GameObject[,] pieces = chessboard.GetPieces();
        List<GameObject> whites = chessboard.GetWhitePieces();
        List<Vector2Int> allPossibleMoves = new List<Vector2Int>();
        foreach (var whiteFig in whites)
        {
            BasePiece whitePiece = whiteFig.GetComponent<BasePiece>();
            List<Vector2Int> possibleMoves = whitePiece.GetPossibleMoves(pieces);
            allPossibleMoves.AddRange(possibleMoves);
        }
        return allPossibleMoves.Count > 0 ? false : true;
    }

    bool checkIfBlackInMate()
    {
        GameObject[,] pieces = chessboard.GetPieces();
        List<GameObject> blacks = chessboard.GetBlackPieces();
        List<Vector2Int> allPossibleMoves = new List<Vector2Int>();
        foreach (var blackFig in blacks)
        {
            BasePiece blackPiece = blackFig.GetComponent<BasePiece>();
            List<Vector2Int> possibleMoves = blackPiece.GetPossibleMoves(pieces);
            allPossibleMoves.AddRange(possibleMoves);
        }
        return allPossibleMoves.Count > 0 ? false : true;
    }

    public bool checkIfBlackInCheck(GameObject[,] pieces)
    {
        List<GameObject> whites = chessboard.GetWhitePieces(pieces);
        Vector2Int blackKingCoords = chessboard.GetBlackKingCoords(pieces);
        foreach (var whiteFig in whites)
        {
            BasePiece whitePiece = whiteFig.GetComponent<BasePiece>();
            List<Vector2Int> possibleMoves = whitePiece.GetPossibleMoves(pieces, true);
            foreach (var possibleMove in possibleMoves)
                if (possibleMove == blackKingCoords)
                    return true;
        }

        return false;
    }

    bool checkIfWhiteInCheck()
    {
        GameObject[,] pieces = chessboard.GetPieces();
        List<GameObject> whites = chessboard.GetBlackPieces();
        Vector2Int blackKingCoords = chessboard.GetWhiteKingCoords();
        foreach (var whiteFig in whites)
        {
            BasePiece whitePiece = whiteFig.GetComponent<BasePiece>();
            List<Vector2Int> possibleMoves = whitePiece.GetPossibleMoves(pieces);
            foreach (var possibleMove in possibleMoves)
                if (possibleMove == blackKingCoords)
                    return true;
        }

        return false;
    }

    public bool checkIfWhiteInCheck(GameObject[,] pieces)
    {
        List<GameObject> whites = chessboard.GetBlackPieces(pieces);
        Vector2Int blackKingCoords = chessboard.GetWhiteKingCoords(pieces);
        foreach (var whiteFig in whites)
        {
            BasePiece whitePiece = whiteFig.GetComponent<BasePiece>();
            List<Vector2Int> possibleMoves = whitePiece.GetPossibleMoves(pieces, true);
            foreach (var possibleMove in possibleMoves)
                if (possibleMove == blackKingCoords)
                    return true;
        }

        return false;
    }

    private void FinalSolution()
    {
        //    
        switch (whoInCheck)
        {
            case TurnEnum.White:
                {
                    if (checkIfWhiteInMate())
                    {
                        WhiteInMate();
                    }
                    break;
                }
            case TurnEnum.Black:
                {
                    if (checkIfBlackInMate())
                    {
                        BlackInMate();
                    }
                    break;
                }
            case TurnEnum.Scenario:
                {
                    switch (curTurn)
                    {
                        case TurnEnum.White:
                            {
                                if (checkIfBlackInMate())
                                    Stalemate();
                                break;

                            }
                        case TurnEnum.Black:
                            {
                                if (checkIfWhiteInMate())
                                    Stalemate();
                                break;
                            }
                    }
                    break;
                }
        }
        Vector2Int notFound = new Vector2Int(-1, -1);
        //     ,   - 
        if (chessboard.GetBlackKingCoords() == notFound)
            BlackInMate();
        else if (chessboard.GetWhiteKingCoords() == notFound)
            WhiteInMate();

        //        
        if (curTurn == TurnEnum.Scenario)
            round++;
        //    
        NextTurn();
    }

    private async Task GradeWhitePawn(Vector2Int coords)
    {
        // Временно отключаем взаимодействие с доской
        curTurn = TurnEnum.NoOne;

        // Предлогаем игроку выбрать тип новой фигуры
        // Реализована логика пользовательского интерфейса, где игрок выберет фигуру
        // Есть GameManager с методом ChoosePieceType()
        GameManager gameManager = FindObjectOfType<GameManager>();
        var taskCompletionSource = new TaskCompletionSource<object>();

        gameManager.ChoosePieceType((newPiece) =>
        {
            Debug.Log("Выбранный тип фигуры: " + newPiece);

            GameObject[,] pieces = chessboard.GetPieces();
            int x = coords.x;
            int y = coords.y;
            GameObject piece = pieces[x, y];
            GameObject newPrefab = null;

            switch (newPiece)
            {
                case BasePiece.PieceType.KNIGHT:
                    newPrefab = chessboard.ChessKnightWhite;
                    break;
                case BasePiece.PieceType.BISHOP:
                    newPrefab = chessboard.ChessBishopWhite;
                    break;
                case BasePiece.PieceType.ROOK:
                    newPrefab = chessboard.ChessRookWhite;
                    break;
                case BasePiece.PieceType.QUEEN:
                    newPrefab = chessboard.ChessQueenWhite;
                    break;
                default:
                    newPrefab = null;
                    break;
            }

            if (newPrefab != null)
            {
                chessboard.RemoveWhitePiece(pieces[x, y]);
                GameObject.Destroy(piece);

                pieces[x, y] = null;
                chessboard.boardLayout.PlacePiece(newPrefab, x, y, BasePiece.PieceState.White, chessboard.transform, chessboard.PIECE_SCALE_FACTOR, chessboard);
            }
            // После завершения выбора фигуры, завершаем Task
            taskCompletionSource.SetResult(null);
        });

        // Ожидаем завершения выбора фигуры
        await taskCompletionSource.Task;

        // Восстанавливаем возможность взаимодействия с доской для хода белых
        curTurn = TurnEnum.White;
    }


    private async Task GradeBlackPawn(Vector2Int coords)
    {
        // Временно отключаем взаимодействие с доской
        curTurn = TurnEnum.NoOne;

        // Предлагаем игроку выбрать тип новой фигуры
        // Реализована логика пользовательского интерфейса, где игрок выберет фигуру
        // Есть GameManager с методом ChoosePieceType()
        GameManager gameManager = FindObjectOfType<GameManager>();
        var taskCompletionSource = new TaskCompletionSource<object>();

        gameManager.ChoosePieceType((newPiece) =>
        {
            Debug.Log("Выбранный тип фигуры: " + newPiece);

            GameObject[,] pieces = chessboard.GetPieces();
            int x = coords.x;
            int y = coords.y;
            GameObject piece = pieces[x, y];
            GameObject newPrefab = null;

            switch (newPiece)
            {
                case BasePiece.PieceType.KNIGHT:
                    newPrefab = chessboard.ChessKnightBlack;
                    break;
                case BasePiece.PieceType.BISHOP:
                    newPrefab = chessboard.ChessBishopBlack;
                    break;
                case BasePiece.PieceType.ROOK:
                    newPrefab = chessboard.ChessRookBlack;
                    break;
                case BasePiece.PieceType.QUEEN:
                    newPrefab = chessboard.ChessQueenBlack;
                    break;
                default:
                    newPrefab = null;
                    break;
            }

            if (newPrefab != null)
            {
                chessboard.RemoveBlackPiece(pieces[x, y]);
                GameObject.Destroy(piece);

                pieces[x, y] = null;
                chessboard.boardLayout.PlacePiece(newPrefab, x, y, BasePiece.PieceState.Black, chessboard.transform, chessboard.PIECE_SCALE_FACTOR, chessboard);
            }
            // После завершения выбора фигуры, завершаем Task
            taskCompletionSource.SetResult(null);
        });

        // Ожидаем завершения выбора фигуры
        await taskCompletionSource.Task;

        // Восстанавливаем возможность взаимодействия с доской для хода черных
        curTurn = TurnEnum.Black;
    }

    public void WhiteInMate()
    {
        endGame = true;
        FindObjectOfType<GameManager>().Checkmate(TurnEnum.Black);
        Debug.Log(" !");
    }

    public void BlackInMate()
    {
        endGame = true;
        FindObjectOfType<GameManager>().Checkmate(TurnEnum.White);
        Debug.Log(" !");
    }

    public void Stalemate()
    {
        stalemateGame = true;
        endGame = true;
        Debug.Log("!");
        FindObjectOfType<GameManager>().Checkmate(TurnEnum.NoOne);
    }

    // возвращает текст - объяснение алгоритма
    public string ExplainAlgo()
    {
        return algo.Explain();
    }

    public void GiveUp()
    {
        switch (curTurn)
        {
            case TurnEnum.White:
                //  1
                StartCoroutine(DelayedCheckmate(TurnEnum.Black, 2f));
                break;
            case TurnEnum.Black:
                //  2 
                StartCoroutine(DelayedCheckmate(TurnEnum.White, 2f));
                break;
        }
    }

    private IEnumerator DelayedCheckmate(TurnEnum winner, float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<GameManager>().Checkmate(winner);
    }

    private void PlaySoundCheck()
    {
        if (audioSource != null && checkSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(checkSound);
        }
    }

    private void PlaySoundMate()
    {
        if (audioSource != null && checkmateSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(checkmateSound);
        }
    }

    private void PlaySoundStalemate()
    {
        if (audioSource != null && drawSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(drawSound);
        }
    }
}
