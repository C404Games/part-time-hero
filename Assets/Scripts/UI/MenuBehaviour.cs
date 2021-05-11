using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;

public class MenuBehaviour : MonoBehaviour
{
    public bool visible = true;
    public bool dishGenerationActive;
    private int level;
    private int numberOfPlayers;
    private List<int> characters;
    private int currentCharacter = 0;
    private float currentTime = 0.0f;
    private float totalTime = 0.0f;
    private int dish1;
    private int dish2;
    private string currentPanelDishName;
    public UnityEngine.UI.Button optionsButton;
    //public UnityEngine.UI.Button returnGameButton;
    //public UnityEngine.UI.Button exitButton;
    //public UnityEngine.UI.Button exitMenuButton;
    public GameObject pauseButton;
    public GameObject panel;
    //public UnityEngine.UI.RawImage menuBackground;
    //public UnityEngine.UI.RawImage exitSceneImage;
    //public UnityEngine.UI.RawImage returnSceneImage;

    public Transform team1PointsPanel;
    public Transform team2PointsPanel;
    public Transform team1dishPanel;
    public Transform team2dishPanel;
    public Text timeText;

    private UnityEngine.UI.Text exitSceneText;
    private UnityEngine.UI.Text returnSceneText;
    public Texture firstPunctuationTexture;
    public Texture secondPunctuationTexture;
    public Texture knifeTexture;
    public Texture spearTexture;
    public Texture ironSwordTexture;
    public Texture woodSwordTexture;
    public Texture carbonatedHoneyTexture;
    public Texture tomatoSoupTexture;
    public Texture hamburgerTexture;
    public Texture cherryBeerTexture;
    public GameObject dishMenuPrefab;
    private Transform teamDishPanel;
    private Transform currentDishPanel;
    //private GameObject searchedDish;
    private int typeOfGame;
    private int frequency = 30;
    //public Texture decorativeElement1;
    //public Texture decorativeElement2;
    MatchManager matchManager;

    // < is called before the first frame update
    void Start()
    {
        //searchedDish = Instantiate(transform.Find("Dish").gameObject);
        /*
        UnityEngine.UI.Button[] buttons = GetComponentsInChildren<UnityEngine.UI.Button>();
        UnityEngine.UI.RawImage[] rawImages = GetComponentsInChildren<UnityEngine.UI.RawImage>();
        returnGameButton = buttons[0];
        exitButton = buttons[1];
        exitMenuButton = buttons[2];
        optionsButton = buttons[3];
        pauseButton = buttons[4];
        optionsButton.onClick.AddListener(openMenu);
        exitMenuButton.onClick.AddListener(closeMenu);
        returnGameButton.onClick.AddListener(closeMenu);
        exitButton.onClick.AddListener(closeMatch);
        pauseButton.onClick.AddListener(pauseMatch);
        menuBackground = rawImages[18];
        exitSceneImage = rawImages[20];
        returnSceneImage = rawImages[21];
        */
        closeMenu();
        //rawImages[9].texture = decorativeElement1;
        //rawImages[10].texture = decorativeElement2;
        matchManager = FindObjectOfType<MatchManager>();
        numberOfPlayers = matchManager.numberOfPlayers;
        typeOfGame = PlayerPrefs.GetInt("tutorialActive", 2);
        if (typeOfGame == 2)
        {
            pauseButton.gameObject.SetActive(false);
        }
        else
        {
            pauseButton.gameObject.SetActive(true);
        }
        optionsButton.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.OfflineMode && PhotonNetwork.LocalPlayer.ActorNumber != 1)
        {
            return;
        }
        else
        {
            if (!matchManager.isPaused)
            {
                CheckForChanges();
                this.gameObject.SetActive(visible);
                currentTime += Time.deltaTime;
                totalTime += Time.deltaTime;
                //Generation of dishes 0s - 30s - 1m 00s...
                if ((currentTime > frequency || matchManager.team1Dishes.Count == 0) && dishGenerationActive)
                {
                    int position = -1;
                    if (matchManager.team1Dishes.Count < 4)
                    {
                        position = matchManager.team1Dishes.Count;
                    }
                    else
                    {
                        for (int i = 0; i < matchManager.team1Dishes.Count; i++)
                        {
                            if (!matchManager.team1Dishes[i].Item1)
                            {
                                position = i;
                                break;
                            }
                        }
                    }
                    position++;
                    Debug.Log("Positions1: " + position);
                    if (position >= 1)
                    {
                        dish1 = matchManager.generateOrder();
                        if (position == (matchManager.team1Dishes.Count + 1))
                        {
                            matchManager.team1Dishes.Add(new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish1].id));
                        }
                        else
                        {
                            matchManager.team1Dishes[position - 1] = new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish1].id);
                        }
                        currentPanelDishName = "Dish" + (position) + "Panel";
                        currentDishPanel = team1dishPanel.Find(currentPanelDishName);
                        dishMenuPrefab = PhotonNetwork.Instantiate(Path.Combine("UI", "Dish"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                        //dishMenuPrefab = Instantiate(searchedDish);
                        dishMenuPrefab.transform.SetParent(currentDishPanel);
                        dishMenuPrefab.transform.position = currentDishPanel.gameObject.transform.position;
                        dishMenuPrefab.transform.SetParent(currentDishPanel);
                        UnityEngine.UI.RawImage rawImageDish = dishMenuPrefab.GetComponentInChildren<UnityEngine.UI.RawImage>();
                        Vector2 panelSize = currentDishPanel.GetComponent<RectTransform>().sizeDelta;
                        rawImageDish.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
                        switch (dish1)
                        {
                            case 19:
                                {
                                    rawImageDish.texture = tomatoSoupTexture;
                                    break;
                                }
                            case 26:
                                {
                                    rawImageDish.texture = hamburgerTexture;
                                    break;
                                }
                            case 15:
                                {
                                    rawImageDish.texture = cherryBeerTexture;
                                    break;
                                }
                            case 13:
                                {
                                    rawImageDish.texture = carbonatedHoneyTexture;
                                    break;
                                }
                        }
                        Text dishNameText = dishMenuPrefab.GetComponentInChildren<Text>();
                        dishNameText.text = ProductManager.finalProducts[dish1].name;
                    }
                    position = -1;
                    if (matchManager.team2Dishes.Count < 4)
                    {
                        position = matchManager.team2Dishes.Count;
                    }
                    else
                    {
                        for (int i = 0; i < matchManager.team2Dishes.Count; i++)
                        {
                            if (!matchManager.team2Dishes[i].Item1)
                            {
                                position = i;
                                break;
                            }
                        }
                    }
                    position++;
                    Debug.Log("Positions2: " + position);
                    if (position >= 1)
                    {
                        dish2 = matchManager.generateOrder();
                        if (position == (matchManager.team2Dishes.Count + 1))
                        {
                            matchManager.team2Dishes.Add(new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish2].id));
                        }
                        else
                        {
                            matchManager.team2Dishes[position - 1] = new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish2].id);
                        }
                        currentPanelDishName = "Dish" + (position) + "Panel";
                        currentDishPanel = team2dishPanel.Find(currentPanelDishName);
                        dishMenuPrefab = PhotonNetwork.Instantiate(Path.Combine("UI", "Dish"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                        //dishMenuPrefab.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000);
                        //dishMenuPrefab = Instantiate(searchedDish);
                        dishMenuPrefab.transform.SetParent(currentDishPanel);
                        dishMenuPrefab.transform.position = currentDishPanel.gameObject.transform.position;
                        dishMenuPrefab.transform.SetParent(currentDishPanel);
                        UnityEngine.UI.RawImage rawImageDish = dishMenuPrefab.GetComponentInChildren<UnityEngine.UI.RawImage>();
                        Vector2 panelSize = currentDishPanel.GetComponent<RectTransform>().sizeDelta;
                        rawImageDish.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
                        switch (dish1)
                        {
                            case 19:
                                {
                                    rawImageDish.texture = tomatoSoupTexture;
                                    break;
                                }
                            case 26:
                                {
                                    rawImageDish.texture = hamburgerTexture;
                                    break;
                                }
                            case 15:
                                {
                                    rawImageDish.texture = cherryBeerTexture;
                                    break;
                                }
                            case 13:
                                {
                                    rawImageDish.texture = carbonatedHoneyTexture;
                                    break;
                                }
                        }
                        UnityEngine.UI.Text dishNameText = dishMenuPrefab.GetComponentInChildren<UnityEngine.UI.Text>();
                        dishNameText.text = ProductManager.finalProducts[dish2].name;
                    }
                    currentTime = 0;
                }

                //Timer countdown
                int minutes = (int)(matchManager.getInitialTime() - totalTime) / 60;
                int seconds = (int)(matchManager.getInitialTime() - totalTime) % 60;
                string minutesStr = (minutes < 10) ? "0" + minutes : "" + minutes;
                string secondsStr = (seconds < 10) ? "0" + seconds : "" + seconds;
                if ((matchManager.getInitialTime() - totalTime) > 0)
                {
                    if (minutes == 0 && seconds >= 15)
                    {
                        if (totalTime % 1 < 0.5)
                        {
                            timeText.color = Color.red;
                        }
                        else
                        {
                            timeText.color = Color.black;
                        }
                    }
                    else if (seconds == 0 || seconds == 30)
                    {
                        timeText.color = Color.red;
                    }
                    else
                    {
                        timeText.color = Color.black;
                    }
                    timeText.text = minutesStr + ":" + secondsStr;
                }
                else
                {
                    timeText.color = Color.red;
                    timeText.text = "00:00";
                    matchManager.updatePlayerMoneyAndExperience();
                    pauseButton.SetActive(false);
                    pauseMatch();
                }

                //PowerUps
                /*
                Transform powerUpPanel = transform.FindChild("PanelPowerUps");
                UnityEngine.UI.RawImage rawImagePowerUp = powerUpPanel.GetComponentInChildren<UnityEngine.UI.RawImage>();
                rawImagePowerUp.enabled = matchManager.getCharactersFrozen()[currentCharacter];
                */
            }
        }
    }

    public void updatePoints()
    {

        //Position of punctuation

        UnityEngine.UI.RawImage[] rawImageTeam1 = team1PointsPanel.GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] textPunctuationTeam1 = team1PointsPanel.GetComponentsInChildren<UnityEngine.UI.Text>();

        UnityEngine.UI.RawImage[] rawImageTeam2 = team2PointsPanel.GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] textPunctuationTeam2 = team2PointsPanel.GetComponentsInChildren<UnityEngine.UI.Text>();

        if (currentCharacter == 0 || currentCharacter == 1)
        {
            if (matchManager.getPunctuationTeam1() > matchManager.getPunctuationTeam2())
            {
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
                textPunctuationTeam2[1].text = "2";
                rawImageTeam2[1].texture = secondPunctuationTexture;
            }
            else if (matchManager.getPunctuationTeam1() == matchManager.getPunctuationTeam2())
            {
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
            else
            {
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam1[1].text = "2";
                rawImageTeam1[1].texture = secondPunctuationTexture;
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
        }
        else
        {
            if (matchManager.getPunctuationTeam1() < matchManager.getPunctuationTeam2())
            {
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam1[1].text = "2";
                rawImageTeam2[1].texture = secondPunctuationTexture;
            }
            else if (matchManager.getPunctuationTeam1() == matchManager.getPunctuationTeam2())
            {
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
                textPunctuationTeam2[1].text = "1";
                rawImageTeam1[1].texture = firstPunctuationTexture;
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
            else
            {
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
                textPunctuationTeam2[1].text = "2";
                rawImageTeam1[1].texture = secondPunctuationTexture;
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam1[1].text = "1";
                rawImageTeam2[1].texture = firstPunctuationTexture;
            }
        }
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

    public void CheckForChanges()
    {
        RectTransform children = transform.GetComponentInChildren<RectTransform>();

        float min_x, max_x, min_y, max_y;
        min_x = max_x = transform.localPosition.x;
        min_y = max_y = transform.localPosition.y;

        foreach (RectTransform child in children)
        {
            Vector2 scale = child.sizeDelta;
            float temp_min_x, temp_max_x, temp_min_y, temp_max_y;

            temp_min_x = child.localPosition.x - (scale.x / 2);
            temp_max_x = child.localPosition.x + (scale.x / 2);
            temp_min_y = child.localPosition.y - (scale.y / 2);
            temp_max_y = child.localPosition.y + (scale.y / 2);

            if (temp_min_x < min_x)
                min_x = temp_min_x;
            if (temp_max_x > max_x)
                max_x = temp_max_x;

            if (temp_min_y < min_y)
                min_y = temp_min_y;
            if (temp_max_y > max_y)
                max_y = temp_max_y;
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(max_x - min_x, max_y - min_y);
    }

    public void openMenu()
    {
        panel.SetActive(true);
        /*
        Color temp = exitMenuButton.GetComponent<UnityEngine.UI.RawImage>().color;
        temp.a = 1.0f;
        exitMenuButton.GetComponent<UnityEngine.UI.RawImage>().color = temp;
        exitMenuButton.interactable = true;
        temp = exitButton.GetComponent<UnityEngine.UI.RawImage>().color;
        temp.a = 1.0f;
        exitButton.GetComponent<UnityEngine.UI.RawImage>().color = temp;
        exitButton.interactable = true;
        temp = returnGameButton.GetComponent<UnityEngine.UI.RawImage>().color;
        temp.a = 1.0f;
        returnGameButton.GetComponent<UnityEngine.UI.RawImage>().color = temp;
        returnGameButton.interactable = true;
        temp = menuBackground.color;
        temp.a = 1.0f;
        exitMenuButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        returnGameButton.gameObject.SetActive(true);
        menuBackground.color = temp;
        menuBackground.gameObject.SetActive(true);
        exitSceneImage.color = temp;
        exitSceneImage.gameObject.SetActive(true);
        returnSceneImage.color = temp;
        returnSceneImage.gameObject.SetActive(true);
        */
    }

    public void closeMenu()
    {
        panel.SetActive(false);
        /*
        Color temp = exitMenuButton.GetComponent<UnityEngine.UI.RawImage>().color;
        temp.a = 0f;
        exitMenuButton.GetComponent<UnityEngine.UI.RawImage>().color = temp;
        exitMenuButton.interactable = false;
        temp = exitButton.GetComponent<UnityEngine.UI.RawImage>().color;
        temp.a = 0f;
        exitButton.GetComponent<UnityEngine.UI.RawImage>().color = temp;
        exitButton.interactable = false;
        temp = returnGameButton.GetComponent<UnityEngine.UI.RawImage>().color;
        temp.a = 0f;
        returnGameButton.GetComponent<UnityEngine.UI.RawImage>().color = temp;
        returnGameButton.interactable = false;
        temp = menuBackground.color;
        temp.a = 0f;
        exitMenuButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        returnGameButton.gameObject.SetActive(false);
        menuBackground.color = temp;
        menuBackground.gameObject.SetActive(false);
        exitSceneImage.color = temp;
        exitSceneImage.gameObject.SetActive(false);
        returnSceneImage.color = temp;
        returnSceneImage.gameObject.SetActive(false);
        /*         
        exitMenuButton.GetComponent<UnityEngine.UI.RawImage>().alpha = 0f;
        exitMenuButton.blocksRaycasts = false;
        exitButton.GetComponent<UnityEngine.UI.RawImage>().alpha = 0f;
        exitButton.blocksRaycasts = false;
        returnGameButton.GetComponent<UnityEngine.UI.RawImage>().alpha = 0f;
        returnGameButton.blocksRaycasts = false;
         */
    }

    public void closeMatch()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void pauseMatch()
    {
        matchManager.isPaused = !matchManager.isPaused;
    }
}
