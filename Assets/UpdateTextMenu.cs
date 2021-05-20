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
    public UnityEngine.UI.Text prologueMainText;
    public UnityEngine.UI.Text prologueDescriptionText;
    public UnityEngine.UI.Text tavernMainText;
    public UnityEngine.UI.Text tavernDescriptionText;
    public UnityEngine.UI.Text smithyMainText;
    public UnityEngine.UI.Text smithyDescriptionText;
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
        UpdateTextOnePlayerMenu();
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
            UpdateTextOnePlayerMenu();
        }
    }

    void UpdateTextOnePlayerMenu()
    {
        string language = PlayerPrefs.GetString("language", "Spanish");
        switch (language)
        {
            case "Spanish":
                {
                    prologueMainText.text = "PRÓLOGO";
                    prologueDescriptionText.text = "Empieza una nueva aventura";
                    tavernMainText.text = "TABERNA";
                    tavernDescriptionText.text = "Tario necesita tu ayuda, ponte manos a la masa";
                    smithyMainText.text = "HERRERÍA";
                    smithyDescriptionText.text = "Ayuda a Rairon en sus tareas al rojo vivo";
                    break;
                }
            case "English":
                {
                    prologueMainText.text = "PROLOGUE";
                    prologueDescriptionText.text = "Start a new adventure";
                    tavernMainText.text = "TAVERN";
                    tavernDescriptionText.text = "Tario needs help, give him a hand in the kitchen";
                    smithyMainText.text = "SMITHY";
                    smithyDescriptionText.text = "Help rairon with his hot smithy tasks";
                    break;
                }
            case "Simplified Chinese":
                {
                    prologueMainText.text = "前言";
                    prologueDescriptionText.text = "開始新的冒險";
                    tavernMainText.text = "酒館";
                    tavernDescriptionText.text = "塔里奧需要幫助，請他幫忙";
                    smithyMainText.text = "史密斯";
                    smithyDescriptionText.text = "幫助Rairon完成他的鐵匠鋪任務";
                    break;
                }
        }
    }
}
