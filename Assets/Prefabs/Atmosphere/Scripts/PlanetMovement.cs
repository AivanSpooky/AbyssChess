using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMovement : MonoBehaviour
{
    // Скорость вращения планеты (градусы в секунду)
    public float rotationSpeed = 10f;

    // Скорость вращения планеты вокруг точки (градусы в секунду)
    public float orbitSpeed = 3f;

    // Центр орбиты
    public Vector3 orbitCenter = new Vector3(4, 0, 4);

    void Update()
    {
        // Вращаем планету вокруг её оси Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Вращаем планету вокруг точки orbitCenter по плоскости XZ
        transform.RotateAround(orbitCenter, Vector3.up, orbitSpeed * Time.deltaTime);
    }
}
