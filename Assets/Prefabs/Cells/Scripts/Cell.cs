using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class cell : MonoBehaviour
{
    public int Column;
    public int Row;
    public Material circleMaterial;
    public void ChangeCoords(int col, int row) {Column = col; Row = row;}
    public enum CellStates
    {
        Default, // ÏÎ ÓÌÎË×ÀÍÈŞ (÷åğíî-áåëûé èëè öâåò àëãîğèòìà)
        Free, // ÑÂÎÁÎÄÍÀß ÊËÅÒÊÀ, ÊÓÄÀ ÌÎÆÍÎ ÑÕÎÄÈÒÜ (çåëåíûé)
        Capture, // ÊËÅÒÊÀ Ñ ÄĞÓÃÎÉ ÔÈÃÓĞÎÉ (êğàñíûé)
        Selected // ÊËÅÒÊÀ Ñ ÑÀÌÎÉ ÔÈÃÓĞÎÉ (æåëòûé)
    }

    public CellStates State = CellStates.Default;
    // ÎĞÈÃÈÍÀËÜÍÛÉ ÖÂÅÒ ÊËÅÒÊÈ (ÁÅËÀß ÈËÈ ×ÅĞÍÀß, ÁÎËÜØÅ ÍÈÊÀÊÀß)
    private Color originalWB;
    public Color getOriginWB() { return originalWB; }
    // ÎĞÈÃÈÍÀËÜÍÛÉ ÖÂÅÒ ÊËÅÒÊÈ ÏĞÈ ÄÅÉÑÒÂÈÈ ÀËÃÎĞÈÒÌÀ (ÀËÃÎĞÈÒÌ ĞÅØÀÅÒ, ÊÀÊÎÉ ÎĞÈÃÈÍÀËÜÍÛÉ ÖÂÅÒ)
    private Color originalColor;
    public void ChangeOriginalColor(Color col)
    {
        originalColor = col;
        ResetColor();
    }

    public void ChangeColor(bool white)
    {
        HighlightTopFace();
        originalWB = white ? Color.white : Color.black;
        originalColor = white ? Color.white : Color.black;
        ChangeColor(white ? Color.white : Color.black, CellStates.Default);
    }

    /*public void ChangeColor(Color col, CellStates state)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = col;
            State = state;
        }
    }*/
    public void ChangeColor(Color col, CellStates state)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.shader.name == "UI/Default")
                {
                    if (col != originalWB)
                    {
                        mat.SetColor("_Color", col);
                    }
                    else
                    {
                        // Óñòàíîâèòü öâåò ïîëíîñòüş ïğîçğà÷íûì
                        Color transparentColor = new Color(0, 0, 0, 0);
                        mat.SetColor("_Color", transparentColor);
                        mat.mainTextureScale = new Vector2(0.8f, 0.8f);
                        mat.mainTextureOffset = new Vector2(0.1f, 0.1f);
                    }
                }
                else
                {
                    if (col == originalWB)
                        mat.color = col;
                }
            }
            State = state;
        }
    }

    public void ResetColor()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && originalColor != null)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.shader.name == "UI/Default")
                {
                    Color transparentColor = new Color(0, 0, 0, 0);
                    mat.SetColor("_Color", transparentColor);
                }
                else
                {
                    mat.color = originalColor;
                }
            }
            renderer.material.color = originalColor;
            State = CellStates.Default;
        }
    }

    public void HighlightTopFace()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                // Íàéòè âåğõíşş ãğàíü ïî íîğìàëè
                if (normals[i] == Vector3.up)
                {
                    mesh.uv[i] = new Vector2(0.5f, 0.5f); // Öåíòğ òåêñòóğû êğóãà
                }
            }

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material[] materials = renderer.materials;
                if (materials.Length > 1)
                {
                    materials[1] = circleMaterial;
                }
                else
                {
                    Array.Resize(ref materials, 2);
                    materials[1] = circleMaterial;
                }
                renderer.materials = materials;
            }
        }
    }
}
