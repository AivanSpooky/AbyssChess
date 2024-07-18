using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Debug.Log("SoundManager Instance created");
        }
        else
        {
            //Debug.Log("Duplicate SoundManager Instance detected and destroyed");
            Destroy(gameObject);
        }

    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            StartCoroutine(PlaySoundCoroutine(clip));

        }
    }

    private IEnumerator PlaySoundCoroutine(AudioClip clip)
    {
        GameObject tempAudioSource = new GameObject("TempAudio");
        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = clip;
        // Установка значений громкости
        audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
        //Debug.Log("Play Sound = " + PlayerPrefs.GetFloat("VolumeSound", 1f));
        audioSource.Play();
        yield return new WaitForSeconds(clip.length);
        Destroy(tempAudioSource);
    }
}
