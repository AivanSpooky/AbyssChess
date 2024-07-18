using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VPScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Text TM; //Создаем зацепки в сцене
    public Button ToExit;

    void Start()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "IntroZero.mp4");
        Debug.Log("И считан путь в переменную filePath   " + filePath + " !!!");
        videoPlayer.url = filePath;
        videoPlayer.Play();

        // Подписываемся на событие завершения видео
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    // Метод, вызываемый по завершении видео для загрузки следующей сцены
    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
