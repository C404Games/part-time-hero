using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMenu : MonoBehaviour
{
    public UnityEngine.UI.Text characterNameText;

    // Start is called before the first frame update
    void Start()
    {
        characterNameText.text = PlayerPrefs.GetString("characterName","");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetString("characterName", characterNameText.text);
    }
}
