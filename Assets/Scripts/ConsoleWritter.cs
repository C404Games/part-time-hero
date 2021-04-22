using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleWritter : MonoBehaviour
{
    public void writeInConsole(string text)
    {
        Debug.Log(text);
    }

    public void writeErrorInConsole(string text)
    {
        Debug.LogError(text);
    }
}
