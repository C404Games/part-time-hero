using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTextMenu : MonoBehaviour
{
    public UnityEngine.UI.Text characterNameText;
    public UnityEngine.UI.Text characterMoneyText;
    public UnityEngine.UI.Text characterLevelText;
    public Animator menuAnimator;
    private string text;


    void Start()
    {
        text = PlayerPrefs.GetString("characterPlayerName", "");
        characterNameText.text = text;
        menuAnimator.SetTrigger("fadeOut");
        text = "" + PlayerPrefs.GetInt("characterMoneyName", 0);
        characterMoneyText.text = text;
        text = "" + getLevelFromExperience(PlayerPrefs.GetInt("characterExperiencesName", 0));
        characterLevelText.text = text;
    }

    private int getLevelFromExperience(int experience)
    {
        int level = 0;
        for(int i = 0; i < 99; i ++)
        {
            if (experience <= (100*level + 500*level*level))
            {
                return level;
            }
            level++;
        }
        return level + 1;
    }
}
