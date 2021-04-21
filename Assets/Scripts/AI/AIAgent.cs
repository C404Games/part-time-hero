using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public AIManager manager;

    public ReachableTracker reachableTracker;

    NavMeshAgent nvAgent;

    ClickMovement movement;

    public RecipieNode currentNode;

    public ProductInstance targetProduct;

    public StationInstance targetStation;

    public DeliverySpot targetDeliverySpot;

    public agentState state = agentState.WAIT;

    public bool busy = false;

    public bool delivering = false;

    // Start is called before the first frame update
    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        movement = GetComponent<ClickMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.targetType == clickTargetType.NONE && !movement.isBlocked())
        {
            switch (state)
            {
                case agentState.FETCH:
                    // SI llegamos al producto
                    if (delivering)
                    {
                        goToTargetDeliverySpot();
                        delivering = false;
                    }
                    else
                    {
                        goToStation(targetStation);
                    }
                    state = agentState.CARRY;
                    break;
                case agentState.CARRY:
                    // Si llegamos al mueble
                    state = agentState.WAIT;
                    busy = false;
                    break;
                case agentState.WAIT:
                    // Espero
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
        else if(targetProduct.getProductType() == ProductType.TOOL)
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
