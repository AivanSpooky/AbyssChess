using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EveryThreeRoundsCycleCellAlgo : Algo
{
    public override void Execute(GameObject[,] tiles, int round)
    {
        int cycles = round / 3;

        if (round % 3 == 0)
        {
            List<int> selectedRows;

            if (cycles < 8)
            {
                selectedRows = new List<int> { 2, 3, 4, 5 };
            }
            else
            {
                selectedRows = new List<int> { 0, 1, 6, 7 };
            }

            foreach (int row in selectedRows)
            {
                var possibleCells = GetNonDeletedTiles(tiles).Where(p =>
                {
                    cell baseTile = p.GetComponent<cell>();
                    return baseTile != null && baseTile.Row == row &&
                           !rememberedCells.Any(rememberedCell =>
                                rememberedCell.Coordinates[0] == baseTile.Column &&
                                rememberedCell.Coordinates[1] == baseTile.Row);
                }).ToList();

                if (possibleCells.Count > 0)
                {
                    int randomIndex = random.Next(possibleCells.Count);
                    GameObject randomTile = possibleCells[randomIndex];
                    cell baseCell = randomTile.GetComponent<cell>();
                    if (baseCell != null)
                    {
                        AlgoCell curAlgoCell = new AlgoCell(new Vector2Int(baseCell.Column, baseCell.Row), random.Next(2, 4));
                        rememberedCells.Add(curAlgoCell);
                        AddRoundsLeftDisplay(curAlgoCell);
                    }
                }
            }
        }
    }

    // возвращает текст - объяснение алгоритма
    public override string Explain()
    {
        return "Every third round, a cell from rows 3 to 6 is selected to disappear after 2-4 rounds. After 24 rounds, cells from any row can be selected.";
    }
}
