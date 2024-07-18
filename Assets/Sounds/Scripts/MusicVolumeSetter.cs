using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeSetter : MonoBehaviour
{
    private AudioSource musicSource; // Аудиоисточник для звуков

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        UpdateVolumeMusic();
    }

    public void UpdateVolumeMusic()
    {
        // Установка значения громкости при запуске
        if (musicSource)
        {
            musicSource.volume = GameManager.VolumeSliderGetPlayer("VolumeMusic");
            //Debug.Log("Music Sound = " + GameManager.VolumeSliderGetPlayer("VolumeMusic"));
        }
    }
}
