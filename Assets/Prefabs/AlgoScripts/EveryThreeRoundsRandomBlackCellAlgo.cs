using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EveryThreeRoundsRandomBlackCellAlgo : Algo
{
    public override void Execute(GameObject[,] tiles, int round)
    {
        if (round % 3 == 0)
        {
            var blackPieces = GetNonDeletedTiles(tiles).Where(p =>
            {
                cell baseTile = p.GetComponent<cell>();
                return baseTile != null && baseTile.getOriginWB() == Color.black &&
                       !rememberedCells.Any(rememberedCell =>
                            rememberedCell.Coordinates[0] == baseTile.Column &&
                            rememberedCell.Coordinates[1] == baseTile.Row);
            }).ToList();

            if (blackPieces.Count > 0)
            {
                int randomIndex = random.Next(blackPieces.Count);
                GameObject randomBlackTile = blackPieces[randomIndex];
                cell baseCell = randomBlackTile.GetComponent<cell>();
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
        return "Every third round, a random undeleted black cell is selected and deleted after 2 rounds.";
    }
}
