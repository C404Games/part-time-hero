using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTextMenu : MonoBehaviour
{
    public UnityEngine.UI.Text characterNameText;
    public Animator menuAnimator;
    private string name;


    void Start()
    {

        name = PlayerPrefs.GetString("characterPlayerName", "");
        characterNameText.text = name;
        menuAnimator.SetTrigger("fadeOut");
    }
}
