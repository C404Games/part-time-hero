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
    public UnityEngine.UI.Text introductionText;
    public UnityEngine.UI.Text introductionText2;
    public UnityEngine.UI.Text textExperiencePostMatch;
    public UnityEngine.UI.Text textMoneyPostMatch;
    public GameObject panelDialogue;
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
    public AudioSource audioClipLoop1;
    public AudioSource audioClipLoop2;
    public AudioSource audioClipLoop3;
    private int currentClip;
    private float currentBackgroundSoundTime;
    private float limitBackgroundSoundTime;
    private bool playingBackgroundClip1;
    private bool playingBackgroundClip2;
    private bool playingBackgroundClip3;
    public Transform team1PointsPanel;
    public Transform team2PointsPanel;
    public Transform team1dishPanel;
    public Transform team2dishPanel;
    public Text timeText;

    public Text moneyFinal;
    public Text pointsFinal;

    private UnityEngine.UI.RawImage team1Dish1Countdown;
    private UnityEngine.UI.RawImage team1Dish2Countdown;
    private UnityEngine.UI.RawImage team1Dish3Countdown;
    private UnityEngine.UI.RawImage team1Dish4Countdown;


    private UnityEngine.UI.RawImage team2Dish1Countdown;
    private UnityEngine.UI.RawImage team2Dish2Countdown;
    private UnityEngine.UI.RawImage team2Dish3Countdown;
    private UnityEngine.UI.RawImage team2Dish4Countdown;

    public UnityEngine.UI.Text exitSceneText;
    public UnityEngine.UI.Text returnSceneText;
    public UnityEngine.UI.Text restartSceneText;
    public UnityEngine.UI.Text pauseSceneText;
    public Texture firstPunctuationTexture;
    public Texture secondPunctuationTexture;
    public GameObject dishMenuPrefab;
    private Transform teamDishPanel;
    private Transform currentDishPanel;
    //private GameObject searchedDish;
    private int typeOfGame;
    private int frequency = 30;
    private bool generalMission;
    //public Texture decorativeElement1;
    //public Texture decorativeElement2;
    MatchManager matchManager;
    private IEnumerator fadeOutCorroutine;
    private IEnumerator fadeOutRestartCorroutine;
    private IEnumerator fadeInCorroutine;
    public UnityEngine.UI.Button pauseButtonInGame;
    public Animator menuAnimator;
    public bool charging;
    public GameObject timeBarTeam1Dish1;
    public GameObject timeBarTeam1Dish2;
    public GameObject timeBarTeam1Dish3;
    public GameObject timeBarTeam1Dish4;
    public GameObject timeBarTeam2Dish1;
    public GameObject timeBarTeam2Dish2;
    public GameObject timeBarTeam2Dish3;
    public GameObject timeBarTeam2Dish4;

    public GameObject dishTeam1Dish1;
    public GameObject dishTeam1Dish2;
    public GameObject dishTeam1Dish3;
    public GameObject dishTeam1Dish4;
    public GameObject dishTeam2Dish1;
    public GameObject dishTeam2Dish2;
    public GameObject dishTeam2Dish3;
    public GameObject dishTeam2Dish4;

    public Gradient gradientCountdown;

    private string language;
    private PhotonView photonView;

    public UnityEngine.UI.Text victoryOrDefeatText;
    public UnityEngine.UI.Text VictoryOrDefeatBackText;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        timeBarTeam1Dish1.SetActive(false);
        timeBarTeam1Dish2.SetActive(false);
        timeBarTeam1Dish3.SetActive(false);
        timeBarTeam1Dish4.SetActive(false);
        timeBarTeam2Dish1.SetActive(false);
        timeBarTeam2Dish2.SetActive(false);
        timeBarTeam2Dish3.SetActive(false);
        timeBarTeam2Dish4.SetActive(false);
        dishTeam1Dish1.SetActive(false);
        dishTeam1Dish2.SetActive(false);
        dishTeam1Dish3.SetActive(false);
        dishTeam1Dish4.SetActive(false);
        dishTeam2Dish1.SetActive(false);
        dishTeam2Dish2.SetActive(false);
        dishTeam2Dish3.SetActive(false);
        dishTeam2Dish4.SetActive(false);
        fadeOutCorroutine = fadeOutScene(1.0f);
        //fadeInCorroutine = fadeInScene(5.0f);
        language = PlayerPrefs.GetString("language", "Spanish");
        if (language == "English")
        {
            exitSceneText.text = "EXIT MENU";
            restartSceneText.text = "RESTART MATCH";
            returnSceneText.text = "RETURN TO THE MATCH";
        }
        else if (language == "Simplified Chinese")
        {
            exitSceneText.text = "退出菜單";
            restartSceneText.text = "重新開始遊戲";
            returnSceneText.text = "回到遊戲";
        }
        if (PlayerPrefs.GetInt("tutorialActive") == 1)
        {
            switch (PlayerPrefs.GetInt("Scenary"))
            {
                case 1:
                    {
                        if (language == "Spanish")
                        {
                            introductionText.text = "Taberna";
                            introductionText2.text = "Taberna";
                        }
                        else if (language == "English")
                        {
                            introductionText.text = "Tavern";
                            introductionText2.text = "Tavern";
                        }
                        else if (language == "Simplified Chinese")
                        {
                            introductionText.text = "酒館";
                            introductionText2.text = "酒館";
                        }
                        //readActionsFromFile(Application.dataPath + "/tabernActions.txt");
                        break;
                    }
                case 2:
                    {
                        if (language == "Spanish")
                        {
                            introductionText.text = "Herrería";
                            introductionText2.text = "Herrería";
                        }
                        else if (language == "English")
                        {
                            introductionText.text = "Smithy";
                            introductionText2.text = "Smithy";
                        }
                        else if (language == "Simplified Chinese")
                        {
                            introductionText.text = "鐵匠鋪";
                            introductionText2.text = "鐵匠鋪";
                        }
                        //readActionsFromFile(Application.dataPath + "/smithyActions.txt");
                        break;
                    }
            }
        }
        if (language == "Spanish")
        {
            pauseSceneText.text = "Pausa";
        }
        else if (language == "English")
        {
            pauseSceneText.text = "Pause";
        }
        else if (language == "Simplified Chinese")
        {
            pauseSceneText.text = "暫停";

        }
    }

        //  is called before the first frame update
        void Start()
    {
        if (PhotonNetwork.OfflineMode)
        {
            pauseButtonInGame.gameObject.SetActive(true);
        }
        else
        {
            pauseButtonInGame.gameObject.SetActive(false);
        }
        StartCoroutine(this.fadeOutCorroutine);
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

            if (PhotonNetwork.OfflineMode)
            {
                pauseButtonInGame.gameObject.SetActive(true);
            }
            else
            {
                pauseButtonInGame.gameObject.SetActive(false);
            }
            if (!matchManager.isPaused && !charging && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("showSceneName") &&
                !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("showTutorial") && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("3_2_1") &&
                !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MaskFadeOutMatchMenu"))
            {
                CheckForChanges();
                currentBackgroundSoundTime += Time.deltaTime;
                int soundTimeMod = (int)(currentBackgroundSoundTime / 8);
                //Play each loop background clip each 8 seconds
                if ((soundTimeMod % 3) == 0 && !playingBackgroundClip1 && currentTime == 0)
                {
                    audioClipLoop1.Play();

                }
                /*else if ((soundTimeMod % 3) == 2 && playingBackgroundClip2)
                {
                    audioClipLoop1.Stop();
                    audioClipLoop2.Stop();
                    audioClipLoop3.Play();
                    playingBackgroundClip1 = false;
                    playingBackgroundClip2 = false;
                    playingBackgroundClip3 = true;
                }
                */
                this.gameObject.SetActive(visible);
                currentTime += Time.deltaTime;
                totalTime += Time.deltaTime;
                photonView.RPC("updateClock", RpcTarget.All, currentTime, totalTime);
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
                    if (position >= 1)
                    {
                        switch (position)
                        {
                            case 1:
                                {
                                    dishTeam1Dish1.SetActive(true);
                                    timeBarTeam1Dish1.SetActive(true);
                                    break;
                                }
                            case 2:
                                {
                                    dishTeam1Dish2.SetActive(true);
                                    timeBarTeam1Dish2.SetActive(true);
                                    break;
                                }
                            case 3:
                                {
                                    dishTeam1Dish3.SetActive(true);
                                    timeBarTeam1Dish3.SetActive(true);
                                    break;
                                }
                            case 4:
                                {
                                    dishTeam1Dish4.SetActive(true);
                                    timeBarTeam1Dish4.SetActive(true);
                                    break;
                                }
                        }
                        dish1 = matchManager.generateOrder();
                        if (position == (matchManager.team1Dishes.Count + 1))
                        {
                            matchManager.team1Dishes.Add(new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish1].id));
                        }
                        else
                        {
                            matchManager.team1Dishes[position - 1] = new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish1].id);
                        }
                        //dishMenuPrefab = Instantiate(searchedDish);
                        photonView.RPC("team1Disk", RpcTarget.All, position, dish1);
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
                    if (position >= 1)
                    {
                        switch (position)
                        {
                            case 1:
                                {
                                    dishTeam2Dish1.SetActive(true);
                                    timeBarTeam2Dish1.SetActive(true);
                                    break;
                                }
                            case 2:
                                {
                                    dishTeam2Dish2.SetActive(true);
                                    timeBarTeam2Dish2.SetActive(true);
                                    break;
                                }
                            case 3:
                                {
                                    dishTeam2Dish3.SetActive(true);
                                    timeBarTeam2Dish3.SetActive(true);
                                    break;
                                }
                            case 4:
                                {
                                    dishTeam2Dish4.SetActive(true);
                                    timeBarTeam2Dish4.SetActive(true);
                                    break;
                                }
                        }
                        dish2 = matchManager.generateOrder();
                        if (position == (matchManager.team2Dishes.Count + 1))
                        {
                            matchManager.team2Dishes.Add(new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish2].id));
                        }
                        else
                        {
                            matchManager.team2Dishes[position - 1] = new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish2].id);
                        }
                        //dishMenuPrefab = PhotonNetwork.Instantiate(Path.Combine("UI", "Dish"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                        photonView.RPC("team2Disk", RpcTarget.All, position, dish2);
                        currentTime = 0;
                    }
                }

                photonView.RPC("clock", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void clock()
    {
        //Timer countdown
        int minutes = (int)(matchManager.getInitialTime() - totalTime) / 60;
        int seconds = (int)(matchManager.getInitialTime() - totalTime) % 60;
        string minutesStr = (minutes < 10) ? "0" + minutes : "" + minutes;
        string secondsStr = (seconds < 10) ? "0" + seconds : "" + seconds;
        if ((matchManager.getInitialTime() - totalTime) > 0)
        {
            if (minutes == 0 && seconds <= 15)
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
            audioClipLoop1.Stop();
            playingBackgroundClip1 = false;
            playingBackgroundClip2 = false;
            playingBackgroundClip3 = false;
            timeText.color = Color.red;
            timeText.text = "00:00";
            matchManager.updatePlayerMoneyAndExperience();
            moneyFinal.text = "" + matchManager.getPunctuationTeam1() / 10;
            pointsFinal.text = "" + matchManager.getPunctuationTeam1();
            pauseButton.SetActive(false);
            pauseMatch();
            GetComponent<Animator>().SetTrigger("fadeIn");
            StartCoroutine(fadeInScene(5.5f));
            string language = PlayerPrefs.GetString("language", "Spanish");
            Debug.Log("PUNCTUATION-----------------" + matchManager.getPunctuationTeam1() + "---" + matchManager.getPunctuationTeam2());
            if (matchManager.getPunctuationTeam1() >= matchManager.getPunctuationTeam2())
            {
                if (language == "Spanish")
                {
                    victoryOrDefeatText.text = "VICTORIA";
                    VictoryOrDefeatBackText.text = "VICTORIA";
                    victoryOrDefeatText.color = Color.red;
                    VictoryOrDefeatBackText.color = Color.black;
                }
                if (language == "English")
                {
                    victoryOrDefeatText.text = "VICTORY";
                    VictoryOrDefeatBackText.text = "VICTORY";
                    victoryOrDefeatText.color = Color.red;
                    VictoryOrDefeatBackText.color = Color.black;
                }
                if (language == "Simplified Chinese")
                {
                    victoryOrDefeatText.text = "勝利";
                    VictoryOrDefeatBackText.text = "勝利";
                    victoryOrDefeatText.color = Color.red;
                    VictoryOrDefeatBackText.color = Color.black;
                }
            } else
            {
                if (language == "Spanish")
                {
                    victoryOrDefeatText.text = "DERROTA";
                    VictoryOrDefeatBackText.text = "DERROTA";
                    victoryOrDefeatText.color = Color.green;
                    VictoryOrDefeatBackText.color = Color.black;
                }
                if (language == "English")
                {
                    victoryOrDefeatText.text = "DEFEAT";
                    VictoryOrDefeatBackText.text = "DEFEAT";
                    victoryOrDefeatText.color = Color.green;
                    VictoryOrDefeatBackText.color = Color.black;
                }
                if (language == "Simplified Chinese")
                {
                    victoryOrDefeatText.text = "打敗";
                    VictoryOrDefeatBackText.text = "打敗";
                    victoryOrDefeatText.color = Color.green;
                    VictoryOrDefeatBackText.color = Color.black;
                }
            }

            // Actualizar nivel desbloqueado
            if (PhotonNetwork.OfflineMode && matchManager.getPunctuationTeam1() > matchManager.getPunctuationTeam2())
            {
                switch (PlayerPrefs.GetInt("scenary", 1))
                {
                    case 1:
                        PlayerPrefs.SetInt("LEVEL_UNLOCKED", 2);
                        break;
                        // Más niveles...
                }
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
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
            }
            else if (matchManager.getPunctuationTeam1() == matchManager.getPunctuationTeam2())
            {
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
            }
            else
            {
                textPunctuationTeam1[0].text = "" + matchManager.getPunctuationTeam1();
                textPunctuationTeam2[0].text = "" + matchManager.getPunctuationTeam2();
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
        matchManager.isPaused = !matchManager.isPaused;
        panel.SetActive(true);
    }

    public void closeMenu()
    {
        panel.SetActive(false);
    }


    public void closeMatch()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void pauseMatch()
    {
        matchManager.isPaused = !matchManager.isPaused;
    }

    public IEnumerator fadeOutScene(float time)
    {
        charging = true;
        menuAnimator.SetTrigger("fadeOut");
        yield return new WaitForSeconds(time);
        menuAnimator.SetTrigger("introduction");
        yield return new WaitForSeconds(9.0f);
        charging = false;
    }

    public IEnumerator fadeInScene(float time)
    {
        yield return new WaitForSeconds(5.0f);
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void reloadLevel()
    {
        int scene = PlayerPrefs.GetInt("Scenary", 0); ;
        if (scene != 0)
        {
            if (scene == 1)
            {
                PhotonNetwork.LoadLevel("Tabern - Level 1");
            }
            else if (scene == 2)
            {
                PhotonNetwork.LoadLevel("Smithy - Level 1");
            }
        }
    }

    public void updateDishStatus(int team, int position, float currentTime, float totalTime)
    {
        currentTime = ((currentTime + currentTime / 10) < totalTime)? currentTime + currentTime/10: totalTime;
        if (team == 1)
        {
            if (position == 0)
            {
                timeBarTeam1Dish1.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam1Dish1.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam1Dish1.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime/totalTime);
            }
            if (position == 1)
            {
                timeBarTeam1Dish2.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam1Dish2.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam1Dish2.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime / totalTime);
            }
            if (position == 2)
            {
                timeBarTeam1Dish3.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam1Dish3.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam1Dish3.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime / totalTime);
            }
            if (position == 3)
            {
                timeBarTeam1Dish4.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam1Dish4.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam1Dish4.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime / totalTime);
            }
        }
        else if (team == 2)
        {
            if (position == 0)
            {
                timeBarTeam2Dish1.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam2Dish1.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam2Dish1.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime / totalTime);
            }
            if (position == 1)
            {
                timeBarTeam2Dish2.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam2Dish2.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam2Dish2.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime / totalTime);
            }
            if (position == 2)
            {
                timeBarTeam2Dish3.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam2Dish3.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam2Dish3.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime / totalTime);
            }
            if (position == 3)
            {
                timeBarTeam2Dish4.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = totalTime;
                timeBarTeam2Dish4.GetComponentInChildren<UnityEngine.UI.Slider>().value = currentTime;
                timeBarTeam2Dish4.GetComponentInChildren<UnityEngine.UI.RawImage>().color = gradientCountdown.Evaluate(currentTime / totalTime);
            }

        }
    }

    [PunRPC]
    void team1Disk(int position, int dish1)
    {
        currentPanelDishName = "Dish" + (position) + "Panel";
        currentDishPanel = team1dishPanel.Find(currentPanelDishName);
        currentDishPanel.gameObject.SetActive(true);
        Image[] imageDish = currentDishPanel.GetComponentsInChildren<Image>(true);
        imageDish[imageDish.Length - 1].sprite = ProductManager.finalProductImage[ProductManager.finalProducts[dish1].id];
        imageDish[imageDish.Length - 1].gameObject.SetActive(true);

        matchManager.team1DishTime[position - 1] = ProductManager.finalProducts[dish1].time;
    }

    [PunRPC]
    void team2Disk(int position, int dish2)
    {

        currentPanelDishName = "Dish" + (position) + "Panel";
        currentDishPanel = team2dishPanel.Find(currentPanelDishName);
        currentDishPanel.gameObject.SetActive(true);
        Image[] imageDish = currentDishPanel.GetComponentsInChildren<Image>(true);
        imageDish[imageDish.Length - 1].sprite = ProductManager.finalProductImage[ProductManager.finalProducts[dish2].id];
        imageDish[imageDish.Length - 1].gameObject.SetActive(true);

        matchManager.team2DishTime[position - 1] = ProductManager.finalProducts[dish2].time;
    }

    [PunRPC]
   void updateClock(float currentTime, float totalTime)
   {
        this.currentTime = currentTime;
        this.totalTime = totalTime;
   }
}