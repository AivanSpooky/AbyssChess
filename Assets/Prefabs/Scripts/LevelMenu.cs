using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LevelMenu : MonoBehaviour
{
    private int cols = 4;
    private int rows = 4;
    private int offsetX = 30;
    private int offsetY = 50;

    public GameObject buttonPrefab; // Префаб кнопки
    public Transform buttonParent;  // Родительский объект для кнопок
    public GameManager gameManager;
    public AudioClip buttonClickSound; // Поле для звука кнопки

    void Awake()
    {
        // Создание кнопок на основе словаря уровней
        CreateLevelButtons();
    }

    public void CreateLevelButtons()
    {
        for (int ind = 0; ind < GameManager.GetCountLevels(); ind++)
        {
            // Создание кнопки
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            // Установка размеров кнопки
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Задание конкретных размеров кнопки (ширина, высота)
                rectTransform.sizeDelta = new Vector2(300, 300);
            }

            // Расположение
            Transform ts = buttonObj.GetComponent<Transform>();
            ts.position = new Vector3(offsetX * (ind % cols) - offsetX*cols/2, ts.position.y + offsetY * (ind / rows), ts.position.z);
            Debug.Log(ts.position);
            Button button = buttonObj.GetComponent<Button>();
            button.name = "LevelButton" + ind;
            
            // Установка текста кнопки
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            // Установка размера шрифта
            buttonText.fontSize = 110;

            if (buttonText != null)
            {
                buttonText.text = (ind + 1).ToString();
            }
            else
            {
                Debug.LogError("No TextMeshPro components found in button prefab");
            }

            // Настройка цветовых состояний кнопки
            ColorBlock cb = button.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.grey; // Цвет при наведении
            cb.pressedColor = Color.black;
            cb.selectedColor = Color.white;
            cb.disabledColor = Color.black;
            button.colors = cb;
            int CompleteLevel = GameManager.GetInfoPlayer("CompleteLevel", 0);
            // Установка активности кнопки
            if (ind > CompleteLevel)
            {
                button.interactable = false;
            }
            Debug.Log("GameManager.CompleteLevel = " + CompleteLevel);

            /// Добавление слушателя для кнопки
            int curint = ind;
            button.onClick.AddListener(() => SetLevelAndStartGame(curint + 1));

            // Добавление компонента ButtonSound и установка звука
            ButtonSound buttonSound = buttonObj.AddComponent<ButtonSound>();
            buttonSound.clickSound = buttonClickSound;
        }
    }

    void SetLevelAndStartGame(int levelIndex)
    {
        Debug.Log(levelIndex);
        // Установка индекса уровня и запуск игры
        GameManager.level = levelIndex;
        gameManager.StartGame();
    }
}
