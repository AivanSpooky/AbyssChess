using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvenRoundRandomCellAlgo : Algo
{
    public override void Execute(GameObject[,] tiles, int round)
    {
        if (round % 2 == 0)
        {
            var nonDeletedPieces = GetNonDeletedTiles(tiles).ToList();
            var availableTiles = nonDeletedPieces
                .Where(tile =>
                {
                    var baseCell = tile.GetComponent<cell>();
                    return baseCell != null &&
                           !rememberedCells.Any(rememberedCell =>
                                rememberedCell.Coordinates[0] == baseCell.Column &&
                                rememberedCell.Coordinates[1] == baseCell.Row);
                })
                .ToList();

            if (availableTiles.Any())
            {
                int randomIndex = random.Next(availableTiles.Count);
                GameObject randomTile = availableTiles[randomIndex];
                cell baseCell = randomTile.GetComponent<cell>();
                if (baseCell != null)
                {
                    AlgoCell curAlgoCell = new AlgoCell(new Vector2Int(baseCell.Column, baseCell.Row), 2);
                    rememberedCells.Add(curAlgoCell);
                    AddRoundsLeftDisplay(curAlgoCell);
                }
            }
        }
    }

    // возвращает текст - объяснение алгоритма
    public override string Explain()
    {
        return "Every even round, a random undeleted cell is selected and deleted after 2 rounds.";
    }
}
