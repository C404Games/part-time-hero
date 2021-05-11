using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class MatchManager : MonoBehaviour
{
    public class Tuple<T, U>
    {
        public T Item1;
        public U Item2;

        public Tuple(T item1, U item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public GameObject team1Part;
    public GameObject team2Part;
    public List<ReachableTracker> trackersTeam1;
    public List<ReachableTracker> trackersTeam2;
    public List<WalkingArea> areasTeam1;
    public List<WalkingArea> areasTeam2;

    public string[] monsterPrefabs;

    private int level;
    private List<PlayerMovement> charactersTeam1;
    private List<PlayerMovement> charactersTeam2;
    private List<StationInstance> stationsTeam1;
    private List<StationInstance> stationsTeam2;
    private float punctuationTeam1;
    private float punctuationTeam2;
    private float initialTime;
    public List<Tuple<bool, int>> team1Dishes;
    public List<Tuple<bool, int>> team2Dishes;
    MenuBehaviour menuBehaviour;
    float freezeTime = 10.0f;
    float fastStationTime = 10.0f;
    float slowStationTime = 10.0f;
    float fastMovementTime = 10.0f;
    public int numberOfPlayers;
    public bool isPaused;

    void Awake()
    {

        team1Dishes = new List<Tuple<bool, int>>();
        team2Dishes = new List<Tuple<bool, int>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        numberOfPlayers = PlayerPrefs.GetInt("numberOfPlayers", 1);
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


        initialTime = 300.0f;

        menuBehaviour = FindObjectOfType<MenuBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            pauseMatch();
        } else
        {            
            unpauseMatch();
        }
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
        Debug.Log("Id: " + randomProduct.id + "Producto: " +randomProduct.name);
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

    public int deliverProduct(int team, int id)
    {
        Tuple<bool, int> deliveryTuple = new Tuple<bool, int>(true,id);
        if (team == 1)
        {
            int value = -1;
            for (int  i = 0; i < team1Dishes.Count; i++)
            {
                if (team1Dishes[i].Item1 && team1Dishes[i].Item2 == id)
                {
                    value = i;
                    break;
                }
            }
            if (value != -1)
            {
                deliveryTuple.Item1 = false;
                team1Dishes[value] = deliveryTuple;
                punctuationTeam1 += (ProductManager.finalProducts[id].difficulty + 1) * 10;
                menuBehaviour.updatePoints();
            }
            return value;
        }
        else if(team == 2)
        {
            int value = -1;
            for (int i = 0; i < team2Dishes.Count; i++)
            {
                if (team2Dishes[i].Item1 && team2Dishes[i].Item2 == id)
                {
                    value = i;
                    break;
                } 
            }
            if (value !=  -1)
            {
                deliveryTuple.Item1 = false;
                team2Dishes[value] = deliveryTuple;
                punctuationTeam2 += (ProductManager.finalProducts[id].difficulty + 1) * 10;
                menuBehaviour.updatePoints();
            }
            return value;
        }
        return -1;
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
                    if (playerMovement.team == 1)
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
                    if (!PhotonNetwork.OfflineMode && PhotonNetwork.LocalPlayer.ActorNumber != 1)
                    {
                        return;
                    }
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
                    string monster = monsterPrefabs[(int)Random.Range(0, monsterPrefabs.Length)];
                    GameObject m = PhotonNetwork.Instantiate(
                        Path.Combine("Monstruos", monster) ,
                        area.RandomPointInBounds(), 
                        Quaternion.identity
                        );
                    m.GetComponent<MonsterController>().reachableTracker = tracker;
                }
                break;
            case PowerupType.STAR:
                {
                    // Cocina instantánea!!!
                    if (playerMovement.team == 1)
                        stationsTeam1.ForEach(p => p.startSpeedChange(0.0f, fastStationTime));
                    else
                        stationsTeam2.ForEach(p => p.startSpeedChange(0.0f, fastStationTime));
                }
                break;
        }

    }

    public void pauseMatch()
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerMovement>().clickMovement.stop();
            players[i].GetComponent<PlayerMovement>().paused = true;
            players[i].GetComponent<PlayerMovement>().iceCube.SetActive(true);
        }
    }

    public void unpauseMatch()
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerMovement>().paused = false;
            players[i].GetComponent<PlayerMovement>().iceCube.SetActive(false);
        }
    }

    public void updatePlayerMoneyAndExperience()
    {
        PlayerPrefs.SetInt("characterExperiencesName", PlayerPrefs.GetInt("characterExperiencesName", 0) + (int)punctuationTeam1 + 100);
        PlayerPrefs.SetInt("characterMoneyName", PlayerPrefs.GetInt("characterMoneyName", 0) + (int)(punctuationTeam1/10) + 10);
    }

}
