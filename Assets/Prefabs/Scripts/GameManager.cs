using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate AlgoCell AlgoFunc(GameObject[,] tiles, int round);

    // Окна
    public GameObject GameDuring;
    public GameObject GamePause;
    public GameObject GameWin;

    public GameObject StartMenu;
    public GameObject LevelMenu;
    public GameObject AlgoExplane;

    public GameObject VictoryImage;
    public GameObject DrawImage;
    public GameObject chessboard;

    public static int level = 0;

    // текстовые поля из настроек
    public TextMeshProUGUI TextCountGames;
    public TextMeshProUGUI TextLevelComplete;
    public TextMeshProUGUI TextCountWinWhite;
    public TextMeshProUGUI TextCountWinBlack;
    public TextMeshProUGUI TextCountDraw;
    // настройки громкости
    public Slider volumeSoundSlider;
    public Slider volumeMusicSlider;
    public TextMeshProUGUI TextVolumeSoundSence;
    public TextMeshProUGUI TextVolumeMusicSence;

    public TextMeshProUGUI TextCurrentTurn;
    public TextMeshProUGUI TextCurLevel;
    public TextMeshProUGUI TextWinner;

    public TextMeshProUGUI TextAlgoExplain;


    public GameObject roundsLeftPrefab;
    private static Dictionary<int, Algo> map;
    public GameObject algo;
    private T CreateAlgoInstance<T>() where T : Algo
    {
        var algoInstance = new GameObject(typeof(T).Name).AddComponent<T>();
        algoInstance.Initialize(roundsLeftPrefab);
        return algoInstance;
    }


    static TurnManager turnManager;

    private void Awake()
    {
        Debug.Log("GameManager.Awake()");
        map = new Dictionary<int, Algo>
        {
            { 1, CreateAlgoInstance<EvenRoundRandomCellAlgo>() },
            { 2, CreateAlgoInstance<EveryThreeRoundsRandomBlackCellAlgo>() },
            { 3, CreateAlgoInstance<EveryFourRoundsSelectTwoCellsAlgo>() },
            { 4, CreateAlgoInstance<EveryThreeRoundsCycleCellAlgo>() }
        };
        Algo algoComp = null;
        if (algo != null)
        {
            algoComp = algo.GetComponent<Algo>();
            level = algoComp.cur_level;
            if (level != 0 && algo != null)
            {
                Debug.Log("Algo found!");
                algoComp.Initialize(algoComp.roundsLeftPrefab);
                turnManager = GetComponent<TurnManager>();
                turnManager.algo = algoComp;
            }
        }
        else
            if (level != 0)
            {
                Debug.Log("Algo picked from map!");
                turnManager = GetComponent<TurnManager>();
                turnManager.algo = map[level];
            }
        if (VictoryImage)
            VictoryImage.SetActive(false); // Скрываем изображение победы
        if (DrawImage)
            DrawImage.SetActive(false); // Скрываем изображение ничьей
        if (TextCurLevel)
            TextCurLevel.text = "Level: " + level;

        if (volumeSoundSlider)
        {
            // устанавливаем запомненные значения слайдеров
            volumeSoundSlider.value = VolumeSliderGetPlayer("VolumeSound");
            TextVolumeSoundSence.text = Mathf.Round(volumeSoundSlider.value * 100) + "%";
            // Добавляем слушатели на изменение значений слайдеров
            volumeSoundSlider.onValueChanged.AddListener(OnVolumeSoundSliderChanged);
        }
        if (volumeMusicSlider)
        {
            // устанавливаем запомненные значения слайдеров
            volumeMusicSlider.value = VolumeSliderGetPlayer("VolumeMusic");
            TextVolumeMusicSence.text = Mathf.Round(volumeMusicSlider.value * 100) + "%";
            // Добавляем слушатели на изменение значений слайдеров
            volumeMusicSlider.onValueChanged.AddListener(OnVolumeMusicSliderChanged);
        }
    }

    // Примерные префабы для выбора типа фигуры
    public UnityEngine.UI.Button knightButton;
    public UnityEngine.UI.Button bishopButton;
    public UnityEngine.UI.Button rookButton;
    public UnityEngine.UI.Button queenButton;

    private BasePiece.PieceType chosenPieceType; // Тип фигуры, выбранный игроком
    private Action<BasePiece.PieceType> onPieceTypeChosen; // Делегат для обратного вызова

    private void Start()
    {
        if (knightButton && bishopButton && rookButton && queenButton)
        {
            knightButton.onClick.AddListener(() => ChoosePiece(BasePiece.PieceType.KNIGHT));
            bishopButton.onClick.AddListener(() => ChoosePiece(BasePiece.PieceType.BISHOP));
            rookButton.onClick.AddListener(() => ChoosePiece(BasePiece.PieceType.ROOK));
            queenButton.onClick.AddListener(() => ChoosePiece(BasePiece.PieceType.QUEEN));

            TogglePieceTypeButtons(false);
        }
    }

    // Метод, который вызывается, когда игрок должен выбрать тип фигуры
    public void ChoosePieceType(Action<BasePiece.PieceType> onPieceChosenCallback)
    {
        onPieceTypeChosen = onPieceChosenCallback;
        TogglePieceTypeButtons(true);
    }

    private void ChoosePiece(BasePiece.PieceType pieceType)
    {
        chosenPieceType = pieceType;
        TogglePieceTypeButtons(false); // Скрываем кнопки после выбора

        if (onPieceTypeChosen != null)
        {
            onPieceTypeChosen(chosenPieceType); // Вызываем делегат с выбранным типом фигуры
        }
    }

    private void TogglePieceTypeButtons(bool show)
    {
        knightButton.gameObject.SetActive(show);
        bishopButton.gameObject.SetActive(show);
        rookButton.gameObject.SetActive(show);
        queenButton.gameObject.SetActive(show);
    }

    // достаёт целочисленное значение по ключу из PlayerPrefs (в отсутствие устанавливает в дефолтное переданное значение)
    public static int GetInfoPlayer(string keyStr, int defSence)
    {
        int tmpNum = defSence;
        if (PlayerPrefs.HasKey(keyStr))
        {
            tmpNum = PlayerPrefs.GetInt(keyStr);
        }
        //Debug.Log("GetInfoPlayer" + keyStr + " " + defSence + " " + tmpNum);
        return tmpNum;
    }

    // устанавливает целочисленное значение по ключу в PlayerPrefs
    public static void SetInfoPlayer(string keyStr, int sence)
    {
        PlayerPrefs.SetInt(keyStr, sence);
        PlayerPrefs.Save();
        //Debug.Log("SetInfoPlayer" + keyStr + " " + sence);
    }

    // выводит в переданное текстовое поле целочисленное значение по ключу из PlayerPrefs
    private void SetInfoPlayerText(TextMeshProUGUI TextPlace, string keyStr, string helpStrLeft, string helpStrRight, int defSence)
    {
        if (TextPlace)
        {
            int tmpNum = GetInfoPlayer(keyStr, defSence);
            TextPlace.text = helpStrLeft + tmpNum + helpStrRight;
            //Debug.Log("SetInfoPlayerText" + keyStr + " " + defSence + " " + tmpNum);
        }
    }

    // изменяет целочисленное значение по ключу на переданное значение в PlayerPrefs
    private void ChangeInfoPlayer(string keyStr, int addNum, int defSence)
    {
        int tmpNum = GetInfoPlayer(keyStr, defSence);
        //Debug.Log("ChangeInfoPlayer" + keyStr + " " + defSence + " " + tmpNum + " + " + addNum);
        tmpNum += addNum;
        SetInfoPlayer(keyStr, tmpNum);
    }

    // запускается при переходе на страницу "О пользователе"
    public void SetInfoAboutPlayer()
    {
        SetInfoPlayerText(TextCountGames, "CountGames", "Number of games played: ", "", 0);
        SetInfoPlayerText(TextLevelComplete, "CompleteLevel", "Complete Level: ", " / " + GetCountLevels(), 0);
        SetInfoPlayerText(TextCountWinWhite, "CountWinWhite", "White: ", "", 0);
        SetInfoPlayerText(TextCountWinBlack, "CountWinBlack", "Black: ", "", 0);
        SetInfoPlayerText(TextCountDraw, "CountDraw", "Stalemate: ", "", 0);
    }

    public void UpdateCurrentTurnText(TurnManager.TurnEnum curTurn)
    {
        if (TextCurrentTurn != null)
        {
            switch (curTurn)
            {
                case TurnManager.TurnEnum.White:
                    TextCurrentTurn.text = "Current Turn: White";
                    break;
                case TurnManager.TurnEnum.Black:
                    TextCurrentTurn.text = "Current Turn: Black";
                    break;
            }
        }
    }

    public void ReturnStartMenu()
    {
        // Запуск корутины с задержкой в 1 секунду
        StartCoroutine(ReturnToStartMenuWithDelay(1f));
    }
    private IEnumerator ReturnToStartMenuWithDelay(float delay)
    {
        // Задержка
        yield return new WaitForSeconds(delay);
        LevelMenu.SetActive(false);
        StartMenu.SetActive(true);
    }

    public void ContinueGame()
    {
        // Запуск корутины с задержкой
        StartCoroutine(ContinueGameWithDelay(1f));
    }
    private IEnumerator ContinueGameWithDelay(float delay)
    {
        // Задержка
        yield return new WaitForSeconds(delay);
        GamePause.SetActive(false);
        GameDuring.SetActive(true);
        chessboard.SetActive(true);
    }

    public void EndPauseGame()
    {
        StartCoroutine(DelayedSceneLoad(1f, - level));
        level = 0;
    }

    public void ExitGame()
    {
        Debug.Log("Quit");
        StartCoroutine(DelayedQuitApplication());
    }

    private IEnumerator DelayedQuitApplication()
    {
        yield return new WaitForSeconds(1.5f);
        Application.Quit();
    }

    public void Checkmate(TurnManager.TurnEnum curTurn)
    {
        switch (curTurn)
        {
            // пишем выигравшего
            case TurnManager.TurnEnum.White:
                TextWinner.text = "Victory\nfor\nwhite";
                ChangeInfoPlayer("CountWinWhite", 1, 0);
                VictoryImage.SetActive(true); // Показываем изображение победы
                DrawImage.SetActive(false); // Скрываем изображение ничьей
                break;
            case TurnManager.TurnEnum.Black:
                TextWinner.text = "Victory\nfor\nblack";
                ChangeInfoPlayer("CountWinBlack", 1, 0);
                VictoryImage.SetActive(true); // Показываем изображение победы
                DrawImage.SetActive(false); // Скрываем изображение ничьей
                break;
            case TurnManager.TurnEnum.NoOne:
                TextWinner.text = "Dead\nheat";
                ChangeInfoPlayer("CountDraw", 1, 0);
                VictoryImage.SetActive(false); // Скрываем изображение победы
                DrawImage.SetActive(true); // Показываем изображение ничьей
                break;
            case TurnManager.TurnEnum.Scenario:
                TextWinner.text = "WTF,\nDisney?!"; break;
        }

        StartCoroutine(DelayedSceneSwitch());
    }

    private IEnumerator DelayedSceneSwitch()
    {
        yield return new WaitForSeconds(2f);

        GameDuring.SetActive(false);
        chessboard.SetActive(false);
        GameWin.SetActive(true);
    }

    public void EndWinnerGame()
    {
        int CompleteLevel = GetInfoPlayer("CompleteLevel", 0);
        if ((CompleteLevel < GetCountLevels()) && (level == (CompleteLevel + 1)))
        {
            // Сохранение нового значения CompleteLevel в PlayerPrefs
            ChangeInfoPlayer("CompleteLevel", 1, 0);
        }
        // Сохранение нового значения CountGames в PlayerPrefs
        ChangeInfoPlayer("CountGames", 1, 0);
        StartCoroutine(DelayedSceneLoad(1.4f, - level));
        level = 0;
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        level = 0;
    }

    public void PlayGame()
    {
        level = GetInfoPlayer("CompleteLevel", 0);
        level += 1;
        if (level == (GetCountLevels() + 1))
        {
            level -= 1;
        }
        StartGame();
    }

    public void StartGame()
    {
        turnManager = GetComponent<TurnManager>();
        turnManager.algo = map[level];
        StartMenu.SetActive(false);
        LevelMenu.SetActive(false);
        AlgoExplane.SetActive(true);
        TextAlgoExplain.text = "Level №" + level + "\n" + turnManager.ExplainAlgo();
    }

    public void LoadGameAfterExplane()
    {
        StartCoroutine(DelayedSceneLoad(0.8f, level));
    }

    private IEnumerator DelayedSceneLoad(float delay, int ind)
    {
        //Debug.Log("DelayedSceneLoad = " + SceneManager.GetActiveScene().buildIndex + ind);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + ind);
    }

    public static int GetCountLevels()
    {
        return map.Count;
    }

    private void OnVolumeSoundSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("VolumeSound", value);
        PlayerPrefs.Save();
        TextVolumeSoundSence.text = Mathf.Round(value * 100) + "%";
    }

    private void OnVolumeMusicSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("VolumeMusic", value);
        PlayerPrefs.Save();
        TextVolumeMusicSence.text = Mathf.Round(value * 100) + "%";
    }

    public static float VolumeSliderGetPlayer(string key)
    {
        float value = 1f; // значение по умолчанию
        if (PlayerPrefs.HasKey(key))
        {
            value = PlayerPrefs.GetFloat(key);
        }
        return value;
    }
}
