using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeInputAction : MonoBehaviour
{
    public TMP_InputField inputField; // Ссылка на TMP_InputField
    public TextMeshProUGUI feedbackText; // Ссылка на TextMeshProUGUI для обратной связи
    public float feedbackDuration = 1.0f; // Продолжительность отображения текста

    void Start()
    {
        // Устанавливаем изначальный текст
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "pedo mellon a minno";

        // Добавляем обработчик события для нажатия Enter
        inputField.onEndEdit.AddListener(HandleCodeInputAction);
    }

    void HandleCodeInputAction(string input)
    {
        // devmode
        if (input == "mellon")
        {
            // Показываем зеленый текст "Код активирован!"
            StartCoroutine(ShowFeedbackText("Code activated!", Color.green, feedbackDuration));


            // Вызываем метод action()
            action();
        }
    }

    IEnumerator ShowFeedbackText(string message, Color color, float duration)
    {
        feedbackText.text = message;
        // Блокируем ввод
        inputField.interactable = false;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        // Очищаем текст в TMP_InputField
        inputField.text = "";
        feedbackText.gameObject.SetActive(false);

        // Разблокируем ввод
        inputField.interactable = true;
    }

    void action()
    {
        Debug.Log("Action method called!");
        GameManager.SetInfoPlayer("CompleteLevel", GameManager.GetCountLevels());
    }
}
