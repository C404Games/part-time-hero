using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTextMenu : MonoBehaviour
{
    public UnityEngine.UI.Text characterNameText;
    public UnityEngine.UI.Text characterMoneyText;
    public UnityEngine.UI.Text characterLevelText;
    public UnityEngine.UI.InputField characterInputFieldNameText;
    public UnityEngine.UI.Dropdown languageDropdown;
    public Animator menuAnimator;
    private string text;


    void Start()
    {
        string name = "Ghost " + Random.Range(0, 10000);
        text = PlayerPrefs.GetString("characterPlayerName", "");
        if (text == ""  || text.ToUpper() == "NOMBRE")
        {
            PlayerPrefs.SetString("characterPlayerName", name) ;
        }
        name = PlayerPrefs.GetString("characterPlayerName", "");
        characterInputFieldNameText.text = name;
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

    private void Update()
    {
        PlayerPrefs.SetString("characterPlayerName", characterInputFieldNameText.text.ToUpper());
        text = "" + PlayerPrefs.GetInt("characterMoneyName", 0);
        characterMoneyText.text = text;
        text = "" + getLevelFromExperience(PlayerPrefs.GetInt("characterExperiencesName", 0));
        characterLevelText.text = text;
        if (languageDropdown.value < languageDropdown.options.Count && (languageDropdown.value != -1 || languageDropdown.value != null))
        {
            PlayerPrefs.SetString("language", languageDropdown.options[languageDropdown.value].text);
        }
    }
}
