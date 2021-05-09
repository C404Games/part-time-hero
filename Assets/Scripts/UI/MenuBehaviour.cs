﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.IO;

public class MenuBehaviour : MonoBehaviour
{
    public bool visible = true;
    public bool dishGenerationActive;
    private int level;
    private int numberOfPlayers = 1;
    private List<int> characters;
    private int currentCharacter = 0;
    private float currentTime = 0.0f;
    private float totalTime = 0.0f;
    private int dish1;
    private int dish2;
    private string currentPanelDishName;
    public Texture firstPunctuationTexture;
    public Texture secondPunctuationTexture;
    public GameObject dishMenuPrefab;
    private Transform teamDishPanel;
    private Transform currentDishPanel;
    public int frequency = 5;
    public Texture decorativeElement1;
    public Texture decorativeElement2;

    MatchManager matchManager;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.UI.RawImage[] rawImages = GetComponentsInChildren<UnityEngine.UI.RawImage>();
        rawImages[9].texture = decorativeElement1;
        rawImages[10].texture = decorativeElement2;
        matchManager = FindObjectOfType<MatchManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(PhotonNetwork.OfflineMode || PhotonNetwork.LocalPlayer.ActorNumber == 1))
        {
            return;
        }
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
            } else
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
                dish1 = matchManager.generateOrder();
                if (position == (matchManager.team1Dishes.Count + 1))
                {
                    matchManager.team1Dishes.Add(new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish1].id));
                }else
                {
                    matchManager.team1Dishes[position-1] = new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish1].id);
                }
                currentPanelDishName = "Dish" + (position) + "Panel";
                teamDishPanel = transform.Find("TeamDish1Panel");
                currentDishPanel = teamDishPanel.Find(currentPanelDishName);
                dishMenuPrefab = PhotonNetwork.Instantiate(Path.Combine("UI", "Dish"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                dishMenuPrefab.transform.SetParent(currentDishPanel);
                dishMenuPrefab.transform.position = currentDishPanel.gameObject.transform.position;
                dishMenuPrefab.transform.SetParent(currentDishPanel);
                UnityEngine.UI.Text dishNameText = dishMenuPrefab.GetComponentInChildren<UnityEngine.UI.Text>();
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
            if (position >= 1)
            {
                dish2 = matchManager.generateOrder();
                if (position == (matchManager.team2Dishes.Count + 1))
                {
                    matchManager.team2Dishes.Add(new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish2].id));
                }
                else
                {
                    matchManager.team2Dishes[position-1] = new MatchManager.Tuple<bool, int>(true, ProductManager.finalProducts[dish2].id);
                }
                currentPanelDishName = "Dish" + (position) + "Panel";
                teamDishPanel = transform.Find("TeamDish2Panel");
                currentDishPanel = teamDishPanel.Find(currentPanelDishName);
                dishMenuPrefab = PhotonNetwork.Instantiate(Path.Combine("UI", "Dish"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                dishMenuPrefab.transform.SetParent(currentDishPanel);
                dishMenuPrefab.transform.position = currentDishPanel.gameObject.transform.position;
                dishMenuPrefab.transform.SetParent(currentDishPanel);
                UnityEngine.UI.Text dishNameText = dishMenuPrefab.GetComponentInChildren<UnityEngine.UI.Text>();
                dishNameText.text = ProductManager.finalProducts[dish2].name;
            }
            currentTime = 0;

        }

        //Timer countdown
        Transform currentTimePanel = transform.Find("TimePanel");
        UnityEngine.UI.Text timeText = currentTimePanel.GetComponentInChildren<UnityEngine.UI.Text>();
        int minutes = (int)(matchManager.getInitialTime() - totalTime) / 60;
        int seconds = (int)(matchManager.getInitialTime() - totalTime) % 60;
        string minutesStr = (minutes < 10) ? "0" + minutes : "" + minutes;
        string secondsStr = (seconds < 10) ? "0" + seconds : "" + seconds;
        if ((matchManager.getInitialTime() - totalTime) > 0 )
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
        }

        //PowerUps
        /*
        Transform powerUpPanel = transform.FindChild("PanelPowerUps");
        UnityEngine.UI.RawImage rawImagePowerUp = powerUpPanel.GetComponentInChildren<UnityEngine.UI.RawImage>();
        rawImagePowerUp.enabled = matchManager.getCharactersFrozen()[currentCharacter];
        */
    }

    public void updatePoints()
    {

        //Position of punctuation

        Transform punctuationPanelTeam1 = transform.Find("PanelPunctuationTeam1");
        UnityEngine.UI.RawImage[] rawImageTeam1 = punctuationPanelTeam1.GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] textPunctuationTeam1 = punctuationPanelTeam1.GetComponentsInChildren<UnityEngine.UI.Text>();

        Transform punctuationPanelTeam2 = transform.Find("PanelPunctuationTeam2");
        UnityEngine.UI.RawImage[] rawImageTeam2 = punctuationPanelTeam2.GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] textPunctuationTeam2 = punctuationPanelTeam2.GetComponentsInChildren<UnityEngine.UI.Text>();

        if (numberOfPlayers == 1 || (numberOfPlayers == 2 && currentCharacter == 0) || (numberOfPlayers == 4 && (currentCharacter == 0 || currentCharacter == 1)))
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
}
