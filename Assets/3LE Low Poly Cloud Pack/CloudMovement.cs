using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    private float originalY;  
    private float amplitude = 0.3f;  
    private float period;

    void Start()
    {
        originalY = transform.position.y;
        SetNewPeriod();
    }

    void Update()
    {
        // Движение облака по синусоиде
        float newY = originalY + amplitude * Mathf.Sin(Time.time * (2 * Mathf.PI / period));
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void SetNewPeriod()
    {
        period = Random.Range(3f, 5f);
    }
}
