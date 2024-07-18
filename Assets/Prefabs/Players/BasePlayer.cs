using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayer: MonoBehaviour
{
    protected Chessboard chessboard;
    public void SetChessboard(Chessboard chessboard) { this.chessboard = chessboard; }
    public abstract void ExecuteAction(Vector2Int oldPos, Vector2Int newPos);

    // Поля для звуков
    public AudioClip movePieceSound;
    public AudioClip capturePieceSound;
    public AudioSource audioSource;

    // Анимации движения фигур вынесены в базовый класс, поскольку вызываются от WhitePlayer и от BlackPlayer
    public enum MoveAnimation
    {
        Default,
        Linear,
        LiftAndPlace
    }
    public static float animationTime = 0.5f;
    public MoveAnimation moveAnimation = MoveAnimation.Default;

    protected IEnumerator MovePieceLinear(GameObject piece, Vector2Int startPos, Vector2Int endPos, float duration)
    {
        Transform transform = piece.GetComponent<Transform>();
        Vector3 startPosition = new Vector3(startPos.x, transform.position.y, startPos.y);
        Vector3 endPosition = new Vector3(endPos.x, transform.position.y, endPos.y);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (transform)
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедиться, что объект точно в конечной позиции
        if (transform)
            transform.position = endPosition;
    }
    protected IEnumerator MovePieceLiftAndPlace(GameObject piece, Vector2Int startPos, Vector2Int endPos, float duration)
    {
        Transform transform = piece.GetComponent<Transform>();
        Vector3 startPosition = new Vector3(startPos.x, transform.position.y, startPos.y);
        Vector3 endPosition = new Vector3(endPos.x, transform.position.y, endPos.y);
        Vector3 liftedPosition = new Vector3((startPos.x + endPos.x) / 2, transform.position.y + 1, (startPos.y + endPos.y) / 2);

        Quaternion startRotation = transform.rotation;
        // ТОНКИЙ МОМЕНТ! КОЭФФИЦИЕНТ ЗДЕСЬ (-1) ИСКЛЮЧИТЕЛЬНО ИЗ-ЗА ТОГО, КАК СДЕЛАНЫ КОНКРЕТНО ВЗЯТЫЕ МНОЙ МОДЕЛЬКИ ФИГУР
        Quaternion lookAtRotation = Quaternion.LookRotation(-1 * (endPosition - startPosition));
        Quaternion endRotation = startRotation;

        float elapsedTime = 0f;

        // Лифтинг вверх и поворот к новой клетке
        while (elapsedTime < duration / 2)
        {
            if (transform)
            {
                transform.position = Vector3.Lerp(startPosition, liftedPosition, (elapsedTime / (duration / 2)));
                transform.rotation = Quaternion.Slerp(startRotation, lookAtRotation, (elapsedTime / (duration / 2)));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Опускаем и возвращаем исходное положение по повороту
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            if (transform)
            {
                transform.position = Vector3.Lerp(liftedPosition, endPosition, (elapsedTime / (duration / 2)));
                transform.rotation = Quaternion.Slerp(lookAtRotation, endRotation, (elapsedTime / (duration / 2)));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедиться, что объект точно в конечной позиции и правильном повороте
        if (transform)
        {
            transform.position = endPosition;
            transform.rotation = endRotation;
        }
    }
    // Конец анимаций

    protected void PlaySoundMove()
    {
        if (audioSource != null && movePieceSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(movePieceSound);
        }
    }
    protected void PlaySoundCapture()
    {
        if (audioSource != null && capturePieceSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(capturePieceSound);
        }
    }
}
