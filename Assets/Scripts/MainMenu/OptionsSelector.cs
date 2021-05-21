using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsSelector : MonoBehaviour
{
    public GameObject movileOptions;
    public GameObject PcOptions;
    public GameObject chest;

    private void Start()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            chest.SetActive(false);
            movileOptions.SetActive(false);
            PcOptions.SetActive(true);
        }
        else if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Other)
        {
            chest.SetActive(false);
            movileOptions.SetActive(true);
            PcOptions.SetActive(false);
        }
    }
}
