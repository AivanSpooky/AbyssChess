using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgoCell
{
    public Vector2Int Coordinates;
    public int RoundsLeft;

    public AlgoCell(Vector2Int coordinates, int roundsLeft)
    {
        Coordinates = coordinates;
        RoundsLeft = roundsLeft;
    }

    public void RoundPassed()
    {
        RoundsLeft--;
    }
}
