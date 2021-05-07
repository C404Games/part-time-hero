using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitLight : MonoBehaviour
{
    public float speed;


    void Update()
    {
        float xAngle = 0;
        float yAngle = 0.05f;
        float zAngle = 0.015f;
        transform.Rotate(xAngle*speed, yAngle*speed, zAngle*speed, Space.Self);
    }
}
