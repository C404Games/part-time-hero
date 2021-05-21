using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeScreenOptions : MonoBehaviour
{
    public Text label;

    public Dropdown dropScreen;
    public Dropdown dropResolution;

    private void Start()
    {
        dropScreen.GetComponent<Dropdown>().value = (Screen.fullScreen) ? 0 : 1;

        switch (Screen.height)
        {
            case 480:
                dropResolution.GetComponent<Dropdown>().value = 0;
                break;
            case 720:
                dropResolution.GetComponent<Dropdown>().value = 1;
                break;
            case 1080:
                dropResolution.GetComponent<Dropdown>().value = 2;
                break;
            case 2160:
                dropResolution.GetComponent<Dropdown>().value = 3;
                break;
        }

    }

    public void changeScreen(int option)
    {
        Debug.Log(label.text);
        if (label.text == "Fullscreen")
        {
            Screen.fullScreen = true;
            dropResolution.interactable = false;
        }
        else
        {
            Screen.fullScreen = false;
            dropResolution.interactable = true;
        }
    }

    public void changeResolution(int option)
    {
        int width = Screen.width;
        int height = Screen.height;

        switch (GetComponent<Dropdown>().value)
        {
            case 3:
                width = 3840;
                height = 2160;
                break;
            case 2:
                width = 1920;
                height = 1080;
                break;
            case 1:
                width = 1280;
                height = 720;
                break;
            case 0:
                width = 852;
                height = 480;
                break;
        }

        Screen.SetResolution(width, height, Screen.fullScreen);
    }
}
