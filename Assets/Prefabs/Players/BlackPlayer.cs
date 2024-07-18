using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WhitePlayer;

public class BlackPlayer : BasePlayer
{
    public override void ExecuteAction(Vector2Int oldPos, Vector2Int newPos)
    {
        GameObject[,] pieces = chessboard.GetPieces();
        // logic
        // Îáìåíèâàåì ıëåìåíòû ìàññèâà
        GameObject temp = pieces[oldPos.x, oldPos.y];

        pieces[oldPos.x, oldPos.y] = pieces[newPos.x, newPos.y];
        pieces[newPos.x, newPos.y] = temp;

        bool firstFigureExists = temp != null;
        bool secondFigureExists = pieces[oldPos.x, oldPos.y] != null;

        // ÌÅÍßŞ ÏÎÇÈÖÈŞ ÏÅĞÂÎÉ ÔÈÃÓĞÛ (×ÅĞÍÎÉ)
        if (firstFigureExists)
        {
            // Àíèìàöèÿ ïåğåäâèæåíèÿ ôèãóğû
            switch (moveAnimation)
            {
                case MoveAnimation.Linear:
                    {
                        StartCoroutine(MovePieceLinear(temp, oldPos, newPos, animationTime));
                        break;
                    }
                case MoveAnimation.Default:
                    {
                        Transform transform = temp.GetComponent<Transform>();
                        transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
                        break;
                    }
                case MoveAnimation.LiftAndPlace:
                    {
                        StartCoroutine(MovePieceLiftAndPlace(temp, oldPos, newPos, animationTime));
                        break;
                    }
                default:
                    {
                        Transform transform = temp.GetComponent<Transform>();
                        transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
                        break;
                    }
            }
            // Çàìåíà ïàğàìåòğîâ íà íîâûå
            BasePiece bp = temp.GetComponent<BasePiece>();
            bp.Row = newPos.y;
            bp.Column = newPos.x;
            bp.Moved = true;
        }

        // ÌÅÍßŞ ÏÎÇÈÖÈŞ ÂÎÇÌÎÆÍÎÉ ÂÒÎĞÎÉ ÔÈÃÓĞÛ (ÍÅ ×ÅĞÍÀß)
        if (secondFigureExists)
        {
            Transform transform = pieces[oldPos.x, oldPos.y].GetComponent<Transform>();
            transform.position = new Vector3(oldPos.x, transform.position.y, oldPos.y);
        }

        // ×ÅĞÍÀß ÔÈÃÓĞÀ ÅÑÒ ÁÅËÓŞ
        if (firstFigureExists && secondFigureExists &&
            Mathf.Abs(pieces[oldPos.x, oldPos.y].GetComponent<BasePiece>().State - temp.GetComponent<BasePiece>().State) == 1)
        {
            chessboard.RemoveWhitePiece(pieces[oldPos.x, oldPos.y]);
            GameObject.Destroy(pieces[oldPos.x, oldPos.y]);
            pieces[oldPos.x, oldPos.y] = null;
            PlaySoundCapture();
        }
        else
        {
            PlaySoundMove();
        }
    }

    public void ExecuteTMPAction(Vector2Int oldPos, Vector2Int newPos, GameObject[,] pieces)
    {
        // Îáìåíèâàåì ıëåìåíòû ìàññèâà
        GameObject temp = pieces[oldPos.x, oldPos.y];

        pieces[oldPos.x, oldPos.y] = pieces[newPos.x, newPos.y];
        pieces[newPos.x, newPos.y] = temp;

        bool firstFigureExists = temp != null;
        bool secondFigureExists = pieces[oldPos.x, oldPos.y] != null;

        // ÌÅÍßŞ ÏÎÇÈÖÈŞ ÏÅĞÂÎÉ ÔÈÃÓĞÛ (×ÅĞÍÎÉ)
        if (firstFigureExists)
        {
            BasePiece bp = temp.GetComponent<BasePiece>();
            bp.Row = newPos.y;
            bp.Column = newPos.x;
            bp.Moved = true;
        }

        // ×ÅĞÍÀß ÔÈÃÓĞÀ ÅÑÒ ÁÅËÓŞ
        if (firstFigureExists && secondFigureExists &&
            Mathf.Abs(pieces[oldPos.x, oldPos.y].GetComponent<BasePiece>().State - temp.GetComponent<BasePiece>().State) == 1)
        {
            GameObject.Destroy(pieces[oldPos.x, oldPos.y]);
            pieces[oldPos.x, oldPos.y] = null;
        }
    }

}
