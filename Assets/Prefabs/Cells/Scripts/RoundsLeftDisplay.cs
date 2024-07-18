using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundsLeftDisplay : MonoBehaviour
{
    public TextMeshPro textMesh;
    public AlgoCell Cell { get; set; }

    private void Start()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }
    }

    public void UpdateText()
    {
        /*Debug.Log($"OK: {textMesh == null} ! {Cell == null}");*/
        if (textMesh != null && Cell != null)
        {
            textMesh.text = Cell.RoundsLeft.ToString();
        }
    }

    private void Update()
    {
        // Обновляем позицию текстового объекта в зависимости от позиции клетки
        if (Cell != null)
            transform.position = new Vector3(Cell.Coordinates.x-0.1f, 0.51f, Cell.Coordinates.y - 0.1f);
        if (FindAnyObjectByType<TurnManager>().curTurn == TurnManager.TurnEnum.Black)
            transform.rotation = Quaternion.Euler(90, 0, 180);
        if (FindAnyObjectByType<TurnManager>().curTurn == TurnManager.TurnEnum.White)
            transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
