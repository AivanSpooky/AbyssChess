using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public abstract class Algo : MonoBehaviour
{
    protected System.Random random = new System.Random();

    public int cur_level;

    protected List<AlgoCell> rememberedCells = new List<AlgoCell>();

    // Префаб для отображения RoundsLeft
    public GameObject roundsLeftPrefab;

    public void Initialize(GameObject prefab)
    {
        this.roundsLeftPrefab = prefab;
    }

    // Список объектов для отображения RoundsLeft
    private List<GameObject> roundsLeftObjects = new List<GameObject>();

    // Правило добавления новой клетки
    public abstract void Execute(GameObject[,] tiles, int round);

    // возвращает текст - объяснение алгоритма
    public abstract string Explain();

    public List<AlgoCell> ExecuteRound(GameObject[,] tiles, int round)
    {
        // Уменьшить кол-во раундов до удаления в каждой клетке списка запомненных клеток
        RoundPassed();
        // Обработка списка запомненных клеток
        return GetAllTiles();
    }

    protected IEnumerable<GameObject> GetNonDeletedTiles(GameObject[,] tiles)
    {
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                yield return tile;
            }
        }
    }

    public void DeleteDeleted()
    {

        List<AlgoCell> cellsToRemove = new List<AlgoCell>();
        List<GameObject> roundObjToRemove = new List<GameObject>();

        // Удаляем клетки из основного списка и удаляем соответствующие текстовые объекты
        for (int i = 0; i < rememberedCells.Count; i++)
        {
            if (rememberedCells[i].RoundsLeft <= 0)
            {
                cellsToRemove.Add(rememberedCells[i]);
                roundObjToRemove.Add(roundsLeftObjects[i]);
            }
        }

        foreach (var cell in cellsToRemove)
        {
            rememberedCells.Remove(cell);
        }
        foreach (var roundObj in roundObjToRemove)
        {
            roundsLeftObjects.Remove(roundObj);
            Destroy(roundObj);
        }
    }

    protected void RoundPassed()
    {
        foreach (var cell in rememberedCells)
        {
            cell.RoundsLeft--;
        }
        UpdateRoundsLeftText();
    }

    protected List<AlgoCell> GetAllTiles()
    {
        return rememberedCells;
    }

    protected void UpdateRoundsLeftText()
    {
        foreach (var roundsLeftObject in roundsLeftObjects)
        {
            var display = roundsLeftObject.GetComponent<RoundsLeftDisplay>();
            if (display != null)
            {
                display.UpdateText();
            }
        }
    }

    protected void AddRoundsLeftDisplay(AlgoCell cell)
    {
        if (roundsLeftPrefab != null)
        {
            var roundsLeftObject = Instantiate(roundsLeftPrefab, new Vector3(cell.Coordinates.x-0.25f, 0.51f, cell.Coordinates.y + 0.4f), Quaternion.Euler(90, 0, 0));
            var display = roundsLeftObject.GetComponent<RoundsLeftDisplay>();
            if (display != null)
            {
                display.Cell = cell;
                display.UpdateText();
            }
            roundsLeftObjects.Add(roundsLeftObject);
            /*Debug.Log(roundsLeftObjects.Count);*/
        }
    }
}




