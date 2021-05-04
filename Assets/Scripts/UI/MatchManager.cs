﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : MonoBehaviour
{

    public GameObject team1Part;
    public GameObject team2Part;

    public List<ReachableTracker> trackersTeam1;
    public List<ReachableTracker> trackersTeam2;

    public List<WalkingArea> areasTeam1;
    public List<WalkingArea> areasTeam2;

    public GameObject[] monsterPrefabs;

    private int level;

    private List<PlayerMovement> charactersTeam1;
    private List<PlayerMovement> charactersTeam2;

    private List<StationInstance> stationsTeam1;
    private List<StationInstance> stationsTeam2;

    private float punctuationTeam1;
    private float punctuationTeam2;

    private float initialTime;

    public List<int> team1Dishes;
    public List<int> team2Dishes;

    MenuBehaviour menuBehaviour;

    float freezeTime = 10.0f;
    float fastStationTime = 10.0f;
    float slowStationTime = 10.0f;
    float fastMovementTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;        

        PlayerMovement[] allCharacters = FindObjectsOfType<PlayerMovement>();
        charactersTeam1 = allCharacters.Where(p => p.team == 1).ToList();
        charactersTeam2 = allCharacters.Where(p => p.team == 2).ToList();

        stationsTeam1 = team1Part.transform.GetComponentsInChildren<StationInstance>().ToList();
        stationsTeam2 = team2Part.transform.GetComponentsInChildren<StationInstance>().ToList();

        /*
        WalkingArea[] allAreas = FindObjectsOfType<WalkingArea>();
        areasTeam1 = allAreas.Where(a => a.teamArea == 1).ToList();
        areasTeam2 = allAreas.Where(a => a.teamArea == 2).ToList();
        */

        team1Dishes = new List<int>();
        team2Dishes = new List<int>();

        initialTime = 300.0f;

        menuBehaviour = FindObjectOfType<MenuBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        //Uncomment when we have more than one player playing at the same time (Photon multiplayer)
        /*
        charactersLife[1] = GameObject.Find("Player2").GetComponent<PlayerMovement>().health;
        if (this.numberOfPlayers == 4)
        {
            charactersLife[2] = GameObject.Find("Player3").GetComponent<PlayerMovement>().health;
            charactersLife[3] = GameObject.Find("Player4").GetComponent<PlayerMovement>().health;
        }
        */
  
        /*
        if (numberOfPlayers == 1)
        {
            this.punctuationTeam1 = GameObject.Find("Player").GetComponent<PlayerMovement>().punctuation;
        }
        else if (numberOfPlayers == 2)
        {
            this.punctuationTeam1 = GameObject.Find("Player").GetComponent<PlayerMovement>().punctuation;
            this.punctuationTeam2 = GameObject.Find("Player2").GetComponent<PlayerMovement>().punctuation;
        }
        else if (numberOfPlayers == 4)
        {
            this.punctuationTeam1 = GameObject.Find("Player").GetComponent<PlayerMovement>().punctuation;
            this.punctuationTeam2 = GameObject.Find("Player3").GetComponent<PlayerMovement>().punctuation;
        }
        */
    }

    public int generateOrder()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        int difficultyLevel = level;
        Product randomProduct = null;
        for (int i = 0; i < level; i++)
        {
            float idx = Random.Range(0.0f, ProductManager.finalProducts.Count);
            List<Product> products = ProductManager.finalProducts.Values.ToList();
            Product product = products[(int)idx];
            if ( i == 0 || difficultyLevel < product.difficulty)
            {
                difficultyLevel = product.difficulty;
                randomProduct = product;
            }
        }
        return randomProduct.id;

    }

    public int getLevel()
    {
        return this.level;

    }

    public float getPunctuationTeam1()
    {
        return this.punctuationTeam1;
    }

    public void setPunctuationTeam1(float punctuationTeam1)
    {
        this.punctuationTeam1 = punctuationTeam1;
    }

    public float getPunctuationTeam2()
    {
        return this.punctuationTeam2;
    }

    public void setPunctuationTeam2(float punctuationTeam2)
    {
        this.punctuationTeam2 = punctuationTeam2;
    }

    public float getInitialTime()
    {
        return this.initialTime;
    }

    public bool deliverProduct(int team, int id)
    {
        if (team == 1 && team1Dishes.Contains(id))
        {
            team1Dishes.Remove(id);
            punctuationTeam1 += (ProductManager.finalProducts[id].difficulty+1) * 10;
            menuBehaviour.updatePoints();
            return true;
        }
        else if(team == 2 && team2Dishes.Contains(id))
        {
            team2Dishes.Remove(id);
            punctuationTeam2 += (ProductManager.finalProducts[id].difficulty + 1) * 10;
            menuBehaviour.updatePoints();
            return true;
        }

        return false;
    }

    public void castPowerup(PlayerMovement playerMovement, PowerupType type)
    {        
        switch (type)
        {
            // Buenos
            case PowerupType.FAST_TIME:
                {
                    // Acelerar cocina
                    if (playerMovement.team == 1)
                        stationsTeam1.ForEach(p => p.startSpeedChange(0.5f, fastStationTime));
                    else
                        stationsTeam2.ForEach(p => p.startSpeedChange(0.5f, fastStationTime));
                }
                break;
            case PowerupType.FAST_WALK:
                {
                    // Acelerar movimiento
                    if(playerMovement.team == 1)
                        charactersTeam1.ForEach(p => p.increaseSpeed(2.0f, fastMovementTime));
                    else
                        charactersTeam2.ForEach(p => p.increaseSpeed(2.0f, fastMovementTime));
                }
                break;

            // Malos
            case PowerupType.SLOW_TIME:
                {
                    // Decelerar cocina
                    if (playerMovement.team == 2)
                        stationsTeam1.ForEach(p => p.startSpeedChange(1.5f, slowStationTime));
                    else
                        stationsTeam2.ForEach(p => p.startSpeedChange(1.5f, slowStationTime));
                }
                break;            
            case PowerupType.FREEZE:
                {
                    // Congelar jugador
                    int team = playerMovement.team;
                    PlayerMovement chosen = null;
                    if (team == 2)
                        chosen = charactersTeam1[(int)Random.Range(0, charactersTeam1.Count)];
                    else
                        chosen = charactersTeam2[(int)Random.Range(0, charactersTeam2.Count)];

                    chosen.freeze(freezeTime);
                }
                break;
            case PowerupType.MONSTER:
                {
                    // Mandar monstruo
                    int team = playerMovement.team;
                    WalkingArea area = null;
                    ReachableTracker tracker = null;
                    if (team == 2)
                    {
                        int idx = Random.Range(0, areasTeam1.Count);
                        area = areasTeam1[idx];
                        tracker = trackersTeam1[idx];
                    }
                    else
                    {
                        int idx = Random.Range(0, areasTeam2.Count);
                        area = areasTeam2[idx];
                        tracker = trackersTeam2[idx];
                    }
                    GameObject monster = monsterPrefabs[(int)Random.Range(0, monsterPrefabs.Length)];
                    monster.GetComponent<MonsterController>().reachableTracker = tracker;
                    Instantiate(monster, area.RandomPointInBounds(), Quaternion.identity);
                }
                break;
        }
    }

}