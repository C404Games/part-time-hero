using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMenuBehaviour : MonoBehaviour
{
    public bool visible = true;
    private bool dishGenerationActive;
    private int level;
    private int numberOfPlayers;
    private List<int> characters;
    private int currentCharacter = 0;
    private float currentTime = 0.0f;
    private int dish;
    public Texture firstPunctuationTexture;
    public Texture secondPunctuationTexture;
    // Start is called before the first frame update
    void Start()
    {
        this.dishGenerationActive = true;
        this.level = GetComponent<MatchManager>().getLevel();
        this.numberOfPlayers = GetComponent<MatchManager>().getNumberOfPlayers();
        this.characters = GetComponent<MatchManager>().getCharacters();
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.SetActive(visible);
        currentTime += Time.deltaTime;

        //Generation of dishes 0s - 30s - 1m 00s...
        if (currentTime % 30 < 0.1 && dishGenerationActive)
        {
            dish = GetComponent<MatchManager>().generateOrder(GetComponent<ProductManager>().getProductBlueprintsLength());
        }
        Transform moneyPanel = GetComponent<Transform>().FindChild("PanelMoney");
        Transform lifePanel = GetComponent<Transform>().FindChild("PanelLife");
        if (moneyPanel != null)
        {
            UnityEngine.UI.Text moneyText = moneyPanel.GetComponentInChildren<UnityEngine.UI.Text>();
            if (currentCharacter == 0 || currentCharacter == 1)
            {
                moneyText.text = "" + GetComponent<MatchManager>().getPunctuationTeam1();
            } else
            {
                moneyText.text = "" + GetComponent<MatchManager>().getPunctuationTeam2();                
            }
        }
        if (lifePanel != null)
        {
            UnityEngine.UI.Text lifeText = lifePanel.GetComponentInChildren<UnityEngine.UI.Text>();
            lifeText.text = "" + GetComponent<MatchManager>().getCharactersLife()[currentCharacter];
        }

        //Timer countdown
        Transform currentTimePanel = GetComponent<Transform>().FindChild("PanelTime");
        UnityEngine.UI.Text timeText = currentTimePanel .GetComponentInChildren<UnityEngine.UI.Text>();
        int minutes = (int)(GetComponent<MatchManager>().getInitialTime() - currentTime) / 60;
        int seconds = (int)(GetComponent<MatchManager>().getInitialTime() - currentTime) % 60;
        string minutesStr = (minutes < 10)? "0" + minutes : "" + minutes;
        string secondsStr = (seconds < 10)? "0" + seconds : "" + seconds;
        if (seconds > 0)
        {
            timeText.text = minutesStr + ":" + secondsStr;
        } else
        {
            timeText.text = "00:00";
        }


        //Position of punctuation

        Transform punctuationPanel = GetComponent<Transform>().FindChild("PanelPunctuation");
        UnityEngine.UI.RawImage rawImage = punctuationPanel.GetComponentInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text textPunctuation = punctuationPanel.GetComponentInChildren<UnityEngine.UI.Text>();
        if (numberOfPlayers == 1 || (numberOfPlayers == 2 && currentCharacter == 0) || (numberOfPlayers == 4 && (currentCharacter == 0 || currentCharacter == 1)))
        {
            if (GetComponent<MatchManager>().getPunctuationTeam1() >= GetComponent<MatchManager>().getPunctuationTeam2())
            {
                textPunctuation.text = "1";
                rawImage.texture = firstPunctuationTexture;
            }
            else
            {
                textPunctuation.text = "2";
                rawImage.texture = secondPunctuationTexture;
            }
        } else 
        {
            if (GetComponent<MatchManager>().getPunctuationTeam2() >= GetComponent<MatchManager>().getPunctuationTeam1())
            {
                textPunctuation.text = "1";
                rawImage.texture = firstPunctuationTexture;
            }
            else
            {
                textPunctuation.text = "2";
                rawImage.texture = secondPunctuationTexture;
            }
        }

        //PowerUps
        Transform powerUpPanel = GetComponent<Transform>().FindChild("PanelPowerUps");
        UnityEngine.UI.RawImage rawImagePowerUp = powerUpPanel.GetComponentInChildren<UnityEngine.UI.RawImage>();
        rawImagePowerUp.enabled = GetComponent<MatchManager>().getCharactersFrozen()[currentCharacter];
    }

    public bool getVisible()
    {
        return this.visible;
    }

    public void getVisible(bool visible)
    {
        this.visible = visible;
    }

}
