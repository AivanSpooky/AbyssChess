using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
        else
        {
            Debug.LogError("Button component is missing on this GameObject.");
        }
    }

    private void PlayClickSound()
    {
        SoundManager soundManager = SoundManager.Instance ?? FindObjectOfType<SoundManager>();
        if (clickSound != null && soundManager != null)
        {
            soundManager.PlaySound(clickSound);
        }
        else
        {
            Debug.LogError("clickSound is null or SoundManager is not found");
        }
    }
}
