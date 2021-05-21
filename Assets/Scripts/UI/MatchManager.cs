using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    public GameObject[] team1DishPanels;
    public GameObject[] team2DishPanels;

    private int level;
    private List<PlayerMovement> charactersTeam1;
    private List<PlayerMovement> charactersTeam2;
    private List<StationInstance> stationsTeam1;
    private List<StationInstance> stationsTeam2;
    [HideInInspector] public float punctuationTeam1;
    [HideInInspector] public float punctuationTeam2;
    [HideInInspector] public int moneyTeam1;
    [HideInInspector] public int moneyTeam2;
    
    [HideInInspector] public List<Tuple<bool, int>> team1Dishes;
    [HideInInspector] public List<Tuple<bool, int>> team2Dishes;

    [HideInInspector] public float[] team1DishTime;
    [HideInInspector] public float[] team2DishTime;

    [HideInInspector] public int numberOfPlayers;
    public bool isPaused;

    MenuBehaviour menuBehaviour;
    float initialTime;
    float freezeTime = 10.0f;
    float fastStationTime = 10.0f;
    float slowStationTime = 10.0f;
    float fastMovementTime = 10.0f;


    void Awake()
    {
        initialTime++;
        team1Dishes = new List<Tuple<bool, int>>();
        team2Dishes = new List<Tuple<bool, int>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Arreglo temporal, deberían de rellenarse automaticamente
        //desde el inpsector. Pero en vez de eso, se vacian por alguna razon.
        // monsterPrefabs[0] = "Ghost_Green Variant";
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

        menuBehaviour = GetComponent<MenuBehaviour>();

        team1DishTime = new float[4];
        team2DishTime = new float[4];
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused || menuBehaviour.charging)
        {
            pauseMatch();
        } else
        {            
            unpauseMatch();
        }

        // Comprobar si se ha acabado el tiempo de las recetas
        for (int i = 0; i < team1Dishes.Count; i++)
        {
            menuBehaviour.updateDishStatus(1, i, team1DishTime[i], ProductManager.finalProducts[team1Dishes[i].Item2].time);
            if (team1Dishes[i] != null && team1Dishes[i].Item1)
            {
                team1DishTime[i] -= Time.deltaTime;
                if (team1DishTime[i] <= 0)
                    deleteOrder(1, team1Dishes[i].Item2);
            }
        }
        for (int i = 0; i < team2Dishes.Count; i++)
        {
            menuBehaviour.updateDishStatus(2, i, team2DishTime[i], ProductManager.finalProducts[team2Dishes[i].Item2].time);
            if (team2Dishes[i] != null && team2Dishes[i].Item1)
            {
                team2DishTime[i] -= Time.deltaTime;
                if (team2DishTime[i] <= 0)
                    deleteOrder(2, team2Dishes[i].Item2);
            }
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
        return randomProduct.id;

    }

    // Devuelve true si había algún pedido con ese id
    public bool deleteOrder(int team, int id)
    {
        List<Tuple<bool, int>> dishes = team == 1? team1Dishes : team2Dishes;
        for(int i = 0; i < dishes.Count; i++)
        {
            Tuple<bool, int> tuple = dishes[i];
            if (tuple.Item1 && tuple.Item2 == id)
            {
                tuple.Item1 = false;
                GameObject dishPanel = team == 1 ? team1DishPanels[i] : team2DishPanels[i];
                Transform currentDishPanel = dishPanel.transform.Find("Image");
                Transform currentTimeBarPanel = dishPanel.transform.Find("GameObject");
                currentDishPanel.gameObject.SetActive(false);
                currentTimeBarPanel.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
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
        if (deleteOrder(team, id))
        {
            if (team == 1)
            {
                punctuationTeam1 += (ProductManager.finalProducts[id].difficulty + 1) * 10;
                moneyTeam1 += (ProductManager.finalProducts[id].difficulty + 1) * 5;
                menuBehaviour.updatePoints();
            }
            else if (team == 2)
            {              
                punctuationTeam2 += (ProductManager.finalProducts[id].difficulty + 1) * 10;
                moneyTeam2 += (ProductManager.finalProducts[id].difficulty + 1) * 5;
                menuBehaviour.updatePoints();
            }
            return true;
        }
        return false;
    }

    [PunRPC]
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
            case PowerupType.MONEY:
                {
                    // Plata o plomo
                    if(playerMovement.team == 1)
                        moneyTeam1 += 10;
                    else
                        moneyTeam2 += 10;                        
                }
                break;
        }

    }

    public void pauseMatch()
    {
        isPaused = true;
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerMovement>().clickMovement.active = false;
            players[i].GetComponent<PlayerMovement>().paused = true;
        }
    }

    public void unpauseMatch()
    {
        isPaused = false;
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerMovement>().active)
            {
                players[i].GetComponent<PlayerMovement>().paused = false;
                players[i].GetComponent<PlayerMovement>().clickMovement.active = true;
            }
        }
    }

    public void restartMatch()
    {
        if (PhotonNetwork.OfflineMode)
        {
            switch (PlayerPrefs.GetInt("scenary"))
            {
                case 1:
                    PhotonNetwork.LoadLevel("Tabern - Level 1");
                    break;
                case 2:
                    PhotonNetwork.LoadLevel("Smithy - Level 1");
                    break;
            }
        }
    }

    public void updatePlayerMoneyAndExperience()
    {
        PlayerPrefs.SetInt("characterExperiencesName", PlayerPrefs.GetInt("characterExperiencesName", 0) + (int)punctuationTeam1);
        PlayerPrefs.SetInt("characterMoneyName", PlayerPrefs.GetInt("characterMoneyName", 0) + moneyTeam1);
    }

    public int getOldestRecipie(int team)
    {
        float[] dishTime = team == 1 ? team1DishTime : team2DishTime;
        List<Tuple<bool, int>> dishes = team == 1 ? team1Dishes : team2Dishes;

        float lowerTime = 999;
        int chosenRecipie = -1;

        for (int i = 0; i < dishes.Count; i++)
        {
            if(dishes[i].Item1 && dishTime[i] < lowerTime && dishTime[i] > 10)
            {
                chosenRecipie = dishes[i].Item2;
            }
        }

        return chosenRecipie;
    }

}
