using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTextMenu : MonoBehaviour
{
    public UnityEngine.UI.Text characterNameText;
    string name;

    void Start()
    {
        name = PlayerPrefs.GetString("characterPlayerName", "");
        characterNameText.text = name;
    }

    void Update()
    {
        characterNameText.text = name;
    }
}
