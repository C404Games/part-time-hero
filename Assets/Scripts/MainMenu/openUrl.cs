using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openUrl : MonoBehaviour
{
    public string URL;

    // Update is called once per frame
    public void open()
    {
        Application.OpenURL(URL);
    }
}
