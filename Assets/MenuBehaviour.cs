using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    public bool visible = true;
    public bool dishGenerationActive;
    private int level;
    private int numberOfPlayers = 1;
    private List<int> characters;
    private int currentCharacter = 0;
    private float currentTime = 0.0f;
    private int dish1;
    private int dish2;
    private string currentPanelDishName;
    public Texture firstPunctuationTexture;
    public Texture secondPunctuationTexture;
    public GameObject dishMenuPrefab;
    private Transform teamDishPanel;
    private Transform currentDishPanel;
    public int frequency = 30;
    public Texture decorativeElement1;
    public Texture decorativeElement2;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.UI.RawImage[] rawImages = GetComponentsInChildren<UnityEngine.UI.RawImage>();
        rawImages[9].texture = decorativeElement1;
        rawImages[10].texture = decorativeElement2;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.SetActive(visible);
        currentTime += Time.deltaTime;

        //Generation of dishes 0s - 30s - 1m 00s...
        if (currentTime > frequency && dishGenerationActive)
        {
            dish1 = GetComponent<MatchManager>().generateOrder();
            dish2 = GetComponent<MatchManager>().generateOrder();
            GetComponent<MatchManager>().team1Dishes.Add(GetComponent<ProductManager>().nonStaticFinalProducts[dish1]);
            if (GetComponent<MatchManager>().team1Dishes.Count <= 4)
            {
                currentPanelDishName = "Dish" + (GetComponent<MatchManager>().team1Dishes.Count) + "Panel";
                teamDishPanel = GetComponent<Transform>().FindChild("TeamDish1Panel");
                currentDishPanel = teamDishPanel.FindChild(currentPanelDishName);
                dishMenuPrefab = Instantiate(GetComponent<Transform>().FindChild("Dish").gameObject) as GameObject;
                dishMenuPrefab.transform.SetParent(currentDishPanel);
                UnityEditor.GameObjectUtility.SetParentAndAlign(dishMenuPrefab, currentDishPanel.gameObject);
                UnityEngine.UI.Text dishNameText = dishMenuPrefab.GetComponentInChildren<UnityEngine.UI.Text>();
                dishNameText.text = GetComponent<ProductManager>().nonStaticFinalProducts[dish1].name;
            }
            GetComponent<MatchManager>().team2Dishes.Add(GetComponent<ProductManager>().nonStaticFinalProducts[dish2]);
            if (GetComponent<MatchManager>().team2Dishes.Count <= 4)
            {
                currentPanelDishName = "Dish" + (GetComponent<MatchManager>().team2Dishes.Count) + "Panel";
                teamDishPanel = GetComponent<Transform>().FindChild("TeamDish2Panel");
                currentDishPanel = teamDishPanel.FindChild(currentPanelDishName);
                dishMenuPrefab = Instantiate(GetComponent<Transform>().FindChild("Dish").gameObject) as GameObject;
                dishMenuPrefab.transform.SetParent(currentDishPanel);
                UnityEditor.GameObjectUtility.SetParentAndAlign(dishMenuPrefab, currentDishPanel.gameObject);
                UnityEngine.UI.Text dishNameText = dishMenuPrefab.GetComponentInChildren<UnityEngine.UI.Text>();
                dishNameText.text = GetComponent<ProductManager>().nonStaticFinalProducts[dish2].name;
            }
            currentTime = 0;
        }

        //Timer countdown
        Transform currentTimePanel = GetComponent<Transform>().FindChild("TimePanel");
        UnityEngine.UI.Text timeText = currentTimePanel.GetComponentInChildren<UnityEngine.UI.Text>();
        int minutes = (int)(GetComponent<MatchManager>().getInitialTime() - currentTime) / 60;
        int seconds = (int)(GetComponent<MatchManager>().getInitialTime() - currentTime) % 60;
        string minutesStr = (minutes < 10) ? "0" + minutes : "" + minutes;
        string secondsStr = (seconds < 10) ? "0" + seconds : "" + seconds;
        if (seconds > 0)
        {
            timeText.text = minutesStr + ":" + secondsStr;
        }
        else
        {
            timeText.text = "00:00";
        }


        //Position of punctuation

        Transform punctuationPanelTeam1 = GetComponent<Transform>().FindChild("PanelPunctuationTeam1");
        UnityEngine.UI.RawImage[] rawImageTeam1 = punctuationPanelTeam1.GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] textPunctuationTeam1= punctuationPanelTeam1.GetComponentsInChildren<UnityEngine.UI.Text>();

        Transform punctuationPanelTeam2 = GetComponent<Transform>().FindChild("PanelPunctuationTeam2");
        UnityEngine.UI.RawImage[] rawImageTeam2 = punctuationPanelTeam2.GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] textPunctuationTeam2= punctuationPanelTeam2.GetComponentsInChildren<UnityEngine.UI.Text>();

        if (numberOfPlayers == 1 || (numberOfPlayers == 2 && currentCharacter == 0) || (numberOfPlayers == 4 && (currentCharacter == 0 || currentCharacter == 1)))
        {
            if (GetComponent<MatchManager>().getPunctuationTeam1() > GetComponent<MatchManager>().getPunctuationTeam2())
            {
                textPunctuationTeam1[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam2[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam2();
                textPunctuationTeam2[1].text = "2";
                rawImageTeam2[1].texture = secondPunctuationTexture;
            }
            else if (GetComponent<MatchManager>().getPunctuationTeam1() == GetComponent<MatchManager>().getPunctuationTeam2())
            {
                textPunctuationTeam1[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam2[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
            else
            {
                textPunctuationTeam1[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam1();
                textPunctuationTeam1[1].text = "2";
                rawImageTeam1[1].texture = secondPunctuationTexture;
                textPunctuationTeam2[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
        }
        else
        {
            if (GetComponent<MatchManager>().getPunctuationTeam1() < GetComponent<MatchManager>().getPunctuationTeam2())
            {
                textPunctuationTeam2[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam1[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam1();
                textPunctuationTeam1[1].text = "2";
                rawImageTeam2[1].texture = secondPunctuationTexture;
            }
            else if (GetComponent<MatchManager>().getPunctuationTeam1() == GetComponent<MatchManager>().getPunctuationTeam2())
            {
                textPunctuationTeam2[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam1[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
            else
            {
                textPunctuationTeam2[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam2();
                textPunctuationTeam2[1].text = "2";
                rawImageTeam1[1].texture = secondPunctuationTexture;
                textPunctuationTeam1[0].text = "" + GetComponent<MatchManager>().getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
        }

        //PowerUps
        /*
        Transform powerUpPanel = GetComponent<Transform>().FindChild("PanelPowerUps");
        UnityEngine.UI.RawImage rawImagePowerUp = powerUpPanel.GetComponentInChildren<UnityEngine.UI.RawImage>();
        rawImagePowerUp.enabled = GetComponent<MatchManager>().getCharactersFrozen()[currentCharacter];
        */
    }

    public bool getVisible()
    {
        return this.visible;
    }

    public void getVisible(bool visible)
    {
        this.visible = visible;
    }

    public void setNumberOfPlayers(int numberOfPlayers)
    {
        this.numberOfPlayers = numberOfPlayers;
    }

    public int getNumberOfPlayers()
    {
        return this.numberOfPlayers;
    }

}
