﻿using UnityEngine;
using UnityEngine.AI;

public enum agentState
{
    FETCH,
    CARRY,
    WAIT
}

public class AIAgent : MonoBehaviour
{

    public int id;

    public GameObject[] characterPrefabs;
    public GameObject modelWrapper;

    AIManager manager;

    public ReachableTracker reachableTracker;

    public CatcherScript catcher;

    NavMeshAgent nvAgent;

    ClickMovement movement;

    public RecipieNode currentNode;

    public ProductInstance targetProduct;

    public StationInstance targetStation;

    public DeliverySpot targetDeliverySpot;

    public agentState state = agentState.WAIT;

    public bool busy = false;

    public bool delivering = false;

    private void Awake()
    {
        // Carga personaje aleatorio
        GameObject model = Instantiate(characterPrefabs[(int)Random.Range(0, characterPrefabs.Length)], modelWrapper.transform);
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        model.GetComponent<SwordVisibility>().playerMovement = playerMovement;
        playerMovement.animator = model.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        movement = GetComponent<ClickMovement>();
        movement.guaranteeTargetProduct = true;

        manager = FindObjectOfType<AIManager>();                
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.targetType == clickTargetType.NONE && !movement.isBlocked() && !movement.isFrozen())
        {
            // SI no, seguimos con la receta
            switch (state)
            {
                case agentState.FETCH:
                    // SI llegamos al producto

                    if (delivering)
                    {
                        goToTargetDeliverySpot();
                    }
                    else
                    {
                        // Comprobamos que efectivamente lo hemos cogido (O se nos ha escapado en la cinta)
                        // SI no, reiniciamos el AIManager
                        if (catcher.getHeldProduct() == null)
                        {
                            manager.resetStep();
                            state = agentState.WAIT;
                        }
                        else
                            goToStation(targetStation);
                    }
                    state = agentState.CARRY;
                    break;
                case agentState.CARRY:
                    // Si llegamos al mueble

                    // Si no hemos podido dejar el objeto, lo dejamos en otro lado
                    if (!delivering && catcher.getHeldProduct() != null)
                    {
                        targetStation = manager.getCommonStation(4);
                        if(targetStation == null)
                            reachableTracker.getStationOnReach(4, false);
                        manager.resetStep();
                        goToStation(targetStation);
                    }
                    else
                        state = agentState.WAIT;
                    break;
                case agentState.WAIT:
                    delivering = false;
                    busy = false;
                    // Si hay monstruo, atacar
                    MonsterController monster = reachableTracker.getNearestMonster();
                    if (monster != null && movement.targetType == clickTargetType.NONE)
                    {
                        movement.targetType = clickTargetType.MONSTER;
                        movement.targetMonster = monster;
                        nvAgent.SetDestination(monster.transform.position);
                        nvAgent.isStopped = false;
                        busy = true;
                    }
                    break;
            }
        }
    }

    public void startBehaviour()
    {
        state = agentState.FETCH;
        busy = true;

        // SI está en mesa
        if (targetProduct.isHeld())
        {

            StationInstance station = targetProduct.getHolder().GetComponent<StationInstance>();
            goToStation(station);
        }
        // Si es herramienta
        else if (targetProduct.getProductType() == ProductType.TOOL)
        {
            ToolSource source = targetProduct.getHolder().GetComponent<ToolSource>();
            movement.targetType = clickTargetType.TOOLSOURCE;
            movement.targetSource = source;
            nvAgent.SetDestination(source.transform.position);
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, nvAgent.destination, NavMesh.AllAreas, path);
            nvAgent.SetPath(path);
            nvAgent.isStopped = false;
        }
        // Está en la cinta
        else
        {
            movement.targetType = clickTargetType.BELT;
            movement.targetProduct = targetProduct;
            nvAgent.SetDestination(targetProduct.transform.position);
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, nvAgent.destination, NavMesh.AllAreas, path);
            nvAgent.SetPath(path);
            nvAgent.isStopped = false;
        }
    }

    private void goToStation(StationInstance station)
    {
        movement.targetType = clickTargetType.STATION;
        movement.targetStation = station;
        Vector3 dest = movement.targetStation.getWaitPos(transform.position);
        nvAgent.SetDestination(dest);
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, nvAgent.destination, NavMesh.AllAreas, path);
        nvAgent.SetPath(path);
        nvAgent.isStopped = false;
    }

    private void goToTargetDeliverySpot()
    {
        movement.targetType = clickTargetType.DELIVERY;
        movement.targetDeliverySpot = targetDeliverySpot;
        Vector3 dest = movement.targetDeliverySpot.transform.position;
        nvAgent.SetDestination(dest);
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, nvAgent.destination, NavMesh.AllAreas, path);
        nvAgent.SetPath(path);
        nvAgent.isStopped = false;
    }
}
