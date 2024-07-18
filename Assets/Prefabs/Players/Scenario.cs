using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Scenario : BasePlayer
{
    public enum TileAnimation
    {
        Default,
        Falling,
        Bubble,
        Exploding,
        Electricution
    }
    public GameObject deletedPiecePrefab;
    public TileAnimation anim;
    public void setAnimation(int level)
    {
        switch (level)
        {
            case 0:
                {
                    anim = TileAnimation.Exploding; break;
                }
            case 1:
                {
                    anim = TileAnimation.Falling; break;
                }
            case 2:
                {
                    anim = TileAnimation.Bubble; break;
                }
            case 3:
                {
                    anim = TileAnimation.Electricution; break;
                }
        }
    }
    public GameObject explosionParticlePrefab;
    public Material darkShaderMaterial;

    // Поля для звуков
    public AudioClip shakingSound;
    public AudioClip fallingSound;
    public AudioClip explosionSound;
    public AudioClip bubbleSound;
    public AudioClip lightningSound;

    private System.Random random = new System.Random();
    public override void ExecuteAction(Vector2Int oldPos, Vector2Int newPos)
    {
        throw new System.NotImplementedException();
    }

    public void ExecuteAction(List<AlgoCell> cells) { ExecuteCells(cells); }

    private void ExecuteCells(List<AlgoCell> cells)
    {
        foreach (var cell in cells)
        {
            ExecuteCell(cell);
        }
    }
    private void ExecuteCell(AlgoCell cell)
    {
        // Логика обработки клетки алгоритма
        if (cell.RoundsLeft == 1)
        {
            chessboard.HighlightCellByCoordsLastRound(cell.Coordinates);
        }
        else if (cell.RoundsLeft == 0) 
        {
            DeleteCell(cell);
        }
        
    }
    
    private void DeleteCell(AlgoCell cell)
    {
        GameObject[,] pieces = chessboard.GetPieces();
        GameObject[,] tiles = chessboard.GetTiles();
        int x = cell.Coordinates[0];
        int y = cell.Coordinates[1];
        GameObject piece = pieces[x, y];
        BasePiece basepiece = null;
        if (piece != null)
            basepiece = piece.GetComponent<BasePiece>();
        GameObject tile = tiles[x, y];

        // Логика удаления клетки
        // Выбор анимации в зависимости от уровня
        switch (anim)
        {
            case TileAnimation.Default:
                StartCoroutine(ExplodeAndDelete(tile, piece));
                GameObject.Destroy(tile);
                break;
            case TileAnimation.Falling:
                StartCoroutine(ShakeAndDelete(tile, piece));
                break;
            case TileAnimation.Bubble:
                StartCoroutine(CreateBubbleAndAnimate(tile, piece));
                break;
            case TileAnimation.Exploding:
                StartCoroutine(ExplodeAndDelete(tile, piece));
                break;
            case TileAnimation.Electricution:
                StartCoroutine(LightningStrike(tile, piece));
                break;
        }
        GameObject.Destroy(tile);
        /*StartCoroutine(ShakeAndDelete(tile, piece));
        TileAnimation anim = TileAnimation.Default;
        switch (anim)
        {
            case TileAnimation.Default:
                {
                    GameObject.Destroy(tile);
                    break;
                }
        }*/
        // Логика удаления фигуры на клетке
        if (basepiece != null)
            switch (basepiece.State)
            {
                case BasePiece.PieceState.White:
                    {
                        chessboard.RemoveWhitePiece(pieces[x, y]);
                        GameObject.Destroy(pieces[x, y]);
                        pieces[x, y] = null;
                        chessboard.boardLayout.PlacePiece(deletedPiecePrefab, x, y, BasePiece.PieceState.Deleted, chessboard.transform, 0f, chessboard);
                        break;
                    }
                case BasePiece.PieceState.Black:
                    {
                        chessboard.RemoveBlackPiece(pieces[x, y]);
                        GameObject.Destroy(pieces[x, y]);
                        pieces[x, y] = null;
                        chessboard.boardLayout.PlacePiece(deletedPiecePrefab, x, y, BasePiece.PieceState.Deleted, chessboard.transform, 0f, chessboard);
                        break;
                    }
                default:
                    {
                        pieces[x, y] = null;
                        chessboard.boardLayout.PlacePiece(deletedPiecePrefab, x, y, BasePiece.PieceState.Deleted, chessboard.transform, 0f, chessboard);
                        break;
                    }
            }
        else
        {
            pieces[x, y] = null;
            chessboard.boardLayout.PlacePiece(deletedPiecePrefab, x, y, BasePiece.PieceState.Deleted, chessboard.transform, 0f, chessboard);
        }


    }

    private IEnumerator ShakeAndDelete(GameObject cell, GameObject piece)
    {
        if (cell != null)
        {
            // Клонировать объекты клетки и фигуры
            GameObject cellClone = Instantiate(cell);
            Destroy(cellClone.GetComponent<cell>());

            GameObject pieceClone = null;
            if (piece != null)
            {
                pieceClone = Instantiate(piece);
                Destroy(pieceClone.GetComponent<BasePiece>());
            }

            // Объединить клонированные объекты в один
            GameObject combinedObject = new GameObject("CombinedObject");
            cellClone.transform.SetParent(combinedObject.transform, false);
            cellClone.transform.localPosition = Vector3.zero; // Установить локальную позицию клетки

            if (pieceClone != null)
            {
                pieceClone.transform.SetParent(combinedObject.transform, false);
                pieceClone.transform.localPosition = piece.transform.localPosition - cell.transform.localPosition; // Корректная установка локальной позиции фигуры
            }

            // Установить позицию объединенного объекта на позицию оригинальной клетки
            combinedObject.transform.position = cell.transform.position;

            // Анимация тряски
            Vector3 originalPosition = combinedObject.transform.position;
            float duration = 1.0f;
            float shakeMagnitude = 0.1f;
            float elapsed = 0.0f;
            PlaySoundShaking();
            while (elapsed < duration)
            {
                float x = originalPosition.x + Random.Range(-1f, 1f) * shakeMagnitude;
                float y = originalPosition.y + Random.Range(-1f, 1f) * shakeMagnitude;
                combinedObject.transform.position = new Vector3(x, y, originalPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Резкое удаление вниз
            float fallDuration = 0.5f;
            Vector3 fallTargetPosition = new Vector3(originalPosition.x, originalPosition.y - 10, originalPosition.z);
            float fallElapsed = 0.0f;
            PlaySoundFalling();
            while (fallElapsed < fallDuration)
            {
                combinedObject.transform.position = Vector3.Lerp(originalPosition, fallTargetPosition, fallElapsed / fallDuration);
                fallElapsed += Time.deltaTime;
                yield return null;
            }

            // Удалить объединенный объект
            Destroy(combinedObject);
        }
    }

    private IEnumerator ExplodeAndDelete(GameObject cell, GameObject piece)
    {
        if (cell != null)
        {
            PlaySoundExplosion();
            // Клонировать объекты клетки и фигуры
            GameObject cellClone = Instantiate(cell);
            Destroy(cellClone.GetComponent<cell>());

            GameObject pieceClone = null;
            if (piece != null)
            {
                pieceClone = Instantiate(piece);
                Destroy(pieceClone.GetComponent<BasePiece>());
            }

            // Объединить клонированные объекты в один
            GameObject combinedObject = new GameObject("CombinedObject");
            cellClone.transform.SetParent(combinedObject.transform, false);
            cellClone.transform.localPosition = Vector3.zero;

            if (pieceClone != null)
            {
                pieceClone.transform.SetParent(combinedObject.transform, false);
                pieceClone.transform.localPosition = piece.transform.localPosition - cell.transform.localPosition;
            }

            combinedObject.transform.position = cell.transform.position;


            // Применить темный шейдер
            Renderer cellRenderer = cellClone.GetComponent<Renderer>();
            Renderer pieceRenderer = pieceClone != null ? pieceClone.GetComponent<Renderer>() : null;

            if (cellRenderer != null)
                StartCoroutine(ApplyDarkShader(cellRenderer, 0.4f));

            if (pieceRenderer != null)
                StartCoroutine(ApplyDarkShader(pieceRenderer, 0.4f));

            // Создание системы частиц для взрыва
            Vector3 explosionPosition = combinedObject.transform.position + new Vector3(0, 1, 0);
            GameObject explosion = Instantiate(explosionParticlePrefab, explosionPosition, Quaternion.identity);
            Destroy(explosion, 1.0f); // Удалить систему частиц через 1 секунду

            // Подождать 1 секунду для завершения взрыва
            yield return new WaitForSeconds(1.0f);
            // Удалить объединенный объект
            Destroy(combinedObject);
        }

    }

    private IEnumerator ApplyDarkShader(Renderer renderer, float duration)
    {
        Material originalMaterial = renderer.material;
        renderer.material = darkShaderMaterial;

        // Дожидаемся завершения анимации
        yield return new WaitForSeconds(duration);

        // Восстанавливаем оригинальный материал (если необходимо)
        // renderer.material = originalMaterial;
    }

    public GameObject bubblePrefab;
    private IEnumerator CreateBubbleAndAnimate(GameObject cell, GameObject piece)
    {
        if (cell != null)
        {
            PlaySoundBubble();
            // Создаем пузырь
            GameObject bubble = Instantiate(bubblePrefab, cell.transform.position, Quaternion.identity);

            // Клонируем объекты клетки и фигуры
            GameObject cellClone = Instantiate(cell);
            Destroy(cellClone.GetComponent<cell>());

            GameObject pieceClone = null;
            if (piece != null)
            {
                pieceClone = Instantiate(piece);
                Destroy(pieceClone.GetComponent<BasePiece>());
            }

            // Объединяем клонированные объекты в один
            GameObject combinedObject = new GameObject("CombinedObject");
            cellClone.transform.SetParent(combinedObject.transform, false);
            cellClone.transform.localPosition = Vector3.zero;

            if (pieceClone != null)
            {
                pieceClone.transform.SetParent(combinedObject.transform, false);
                pieceClone.transform.localPosition = piece.transform.localPosition - cell.transform.localPosition;
            }

            combinedObject.transform.position = cell.transform.position;
            combinedObject.transform.SetParent(bubble.transform, true);

            // Начинаем вращение
            float elapsedTime = 0f;
            float duration = 1f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float randomX = Random.Range(-30f, 30f);
                float randomY = Random.Range(-30f, 30f);
                float randomZ = Random.Range(-30f, 30f);

                combinedObject.transform.Rotate(new Vector3(randomX, randomY, randomZ) * Time.deltaTime);

                yield return null;
            }

            // Улетание вверх
            Vector3 targetPosition = bubble.transform.position + new Vector3(0, 10, 0);
            elapsedTime = 0f;
            duration = 2f;

            Vector3 startPosition = bubble.transform.position;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                bubble.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                yield return null;
            }

            // Удаление объектов
            Destroy(combinedObject);
            Destroy(bubble);
        }
    }

    public GameObject lightningPrefab;
    public Material elecMaterial;
    private IEnumerator LightningStrike(GameObject cell, GameObject piece)
    {
        if (cell != null)
        {
            // Клонировать объекты клетки и фигуры
            GameObject cellClone = Instantiate(cell);
            Destroy(cellClone.GetComponent<cell>());

            GameObject pieceClone = null;
            if (piece != null)
            {
                pieceClone = Instantiate(piece);
                Destroy(pieceClone.GetComponent<BasePiece>());
            }

            // Объединить клонированные объекты в один
            GameObject combinedObject = new GameObject("CombinedObject");
            cellClone.transform.SetParent(combinedObject.transform, false);
            cellClone.transform.localPosition = Vector3.zero;

            if (pieceClone != null)
            {
                pieceClone.transform.SetParent(combinedObject.transform, false);
                pieceClone.transform.localPosition = piece.transform.localPosition - cell.transform.localPosition;
            }

            combinedObject.transform.position = cell.transform.position;

            // Создание эффекта молнии
            Vector3 lightningEndPosition = combinedObject.transform.position;
            Vector3 lightningStartPosition = lightningEndPosition + new Vector3(0, 0, 0); // Высокая точка над клеткой
            GameObject lightning = Instantiate(lightningPrefab, lightningStartPosition, Quaternion.Euler(0, 0, 90));

            // Позиционирование и направление молнии
            lightning.transform.SetParent(combinedObject.transform, false);
            lightning.transform.localPosition = new Vector3(0, 3, 0.15f); // Смещение вниз на половину длины молнии (если высота молнии 10)

            // Воспроизведение звука молнии
            PlaySoundLightning();

            Renderer cellRenderer = cellClone.GetComponent<Renderer>();
            if (cellRenderer != null)
            {
                cellRenderer.material = elecMaterial;
            }

            if (pieceClone != null)
            {
                Renderer pieceRenderer = pieceClone.GetComponent<Renderer>();
                if (pieceRenderer != null)
                {
                    pieceRenderer.material = elecMaterial;
                }
            }

            // Анимация молнии (удар за 0.2 секунды)
            float lightningDuration = 0.2f;
            float elapsed = 0.0f;
            while (elapsed < lightningDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Временное освещение клеток рядом
            Light light = lightning.AddComponent<Light>();
            light.intensity = 5;
            light.range = 3;
            light.color = Color.white;

            // Подождать оставшееся время (чтобы общая анимация длилась 1 секунду)
            yield return new WaitForSeconds(0.8f);

            // Удалить освещение и молнию
            Destroy(lightning);

            // Удалить объединенный объект
            Destroy(combinedObject);
        }
    }

    private void PlaySoundLightning()
    {
        if (audioSource != null && lightningSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(lightningSound);
        }
    }
    private void PlaySoundBubble()
    {
        if (audioSource != null && bubbleSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(bubbleSound);
        }
    }

    private void PlaySoundShaking()
    {
        if (audioSource != null && shakingSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(shakingSound);
        }
    }

    private void PlaySoundFalling()
    {
        if (audioSource != null && fallingSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(fallingSound);
        }
    }

    private void PlaySoundExplosion()
    {
        if (audioSource != null && explosionSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumeSound", 1f);
            audioSource.PlayOneShot(explosionSound);
        }
    }
}
