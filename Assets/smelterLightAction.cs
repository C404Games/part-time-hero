using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smelterLightAction : MonoBehaviour
{

    public Light redLight;
    public Light yellowLight;
    public int number;

    // Update is called once per frame
    void Update()
    {
        redLight.intensity = Mathf.PingPong(Time.time, number + 5);
        yellowLight.intensity = Mathf.PingPong(Time.time, number + 2f);

    }
}
