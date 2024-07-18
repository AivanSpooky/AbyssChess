using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VPPScript : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public Text TM; //Создаем зацепки в сцене
    public Button ToExit;

    void Start()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "IntroZero.mp4");
        Debug.Log("И считан путь в переменную filePath   " + filePath + " !!!");
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "IntroZero.mp4");
        videoPlayer.Play();
    }
}
