using UnityEngine;
using UnityEngine.UI;

public class ImageSound : MonoBehaviour
{
    public AudioClip appearSound;

    private void OnEnable()
    {
        PlayAppearSound();
    }

    private void PlayAppearSound()
    {
        if (appearSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(appearSound);
        }
    }
}
