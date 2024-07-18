using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EveryFourRoundsSelectTwoCellsAlgo : Algo
{
    public override void Execute(GameObject[,] tiles, int round)
    {
        if (round % 4 == 0)
        {
            var selectedRows = new List<int> { 2, 3, 4, 5 };
            var possibleCells = GetNonDeletedTiles(tiles).Where(p =>
            {
                cell baseTile = p.GetComponent<cell>();
                return baseTile != null && selectedRows.Contains(baseTile.Row) &&
                       !rememberedCells.Any(rememberedCell =>
                            rememberedCell.Coordinates[0] == baseTile.Column &&
                            rememberedCell.Coordinates[1] == baseTile.Row);
            }).ToList();

            if (possibleCells.Count > 0)
            {
                int randomIndex1 = random.Next(possibleCells.Count);
                GameObject randomTile1 = possibleCells[randomIndex1];
                cell baseCell1 = randomTile1.GetComponent<cell>();
                if (baseCell1 != null)
                {
                    AlgoCell curAlgoCell1 = new AlgoCell(new Vector2Int(baseCell1.Column, baseCell1.Row), random.Next(2, 4));
                    rememberedCells.Add(curAlgoCell1);
                    AddRoundsLeftDisplay(curAlgoCell1);
                }

                // Remove the first selected cell from the list to avoid selecting it again
                possibleCells.RemoveAt(randomIndex1);

                if (possibleCells.Count > 0)
                {
                    int randomIndex2 = random.Next(possibleCells.Count);
                    GameObject randomTile2 = possibleCells[randomIndex2];
                    cell baseCell2 = randomTile2.GetComponent<cell>();
                    if (baseCell2 != null)
                    {
                        AlgoCell curAlgoCell2 = new AlgoCell(new Vector2Int(baseCell2.Column, baseCell2.Row), random.Next(2, 4));
                        rememberedCells.Add(curAlgoCell2);
                        AddRoundsLeftDisplay(curAlgoCell2);
                    }
                }
            }
        }
    }
    // возвращает текст - объяснение алгоритма
    public override string Explain()
    {
        return "Every fourth round, two random cells from rows 3 to 6 are selected and deleted after 2-4 rounds.";
    }
}
