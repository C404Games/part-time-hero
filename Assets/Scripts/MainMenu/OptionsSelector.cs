using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsSelector : MonoBehaviour
{
    public GameObject movileOptions;
    public GameObject PcOptions;

    private void Start()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            movileOptions.SetActive(false);
            PcOptions.SetActive(true);
        }
        else if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Other)
        {
            Debug.Log("Otro");
            movileOptions.SetActive(true);
            PcOptions.SetActive(false);
        }
    }
}
