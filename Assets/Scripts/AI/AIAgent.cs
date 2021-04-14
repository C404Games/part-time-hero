using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    FETCH,
    CARRY
}

public class AIAgent : MonoBehaviour
{

    bool busy = false;

    public AIState state = AIState.FETCH;

    public AIManager manager;

    public ReachableTracker reachableTracker;

    NavMeshAgent nvAgent;

    ClickMovement movement;

    RecipieNode currentNode;

    StationInstance trackedStation;    

    // Start is called before the first frame update
    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        movement = GetComponent<ClickMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.targetType == clickTargetType.NONE)
        {
            switch (state)
            {
                case AIState.FETCH:
                    fetchProduct();
                    break;
                case AIState.CARRY:
                    carryProduct();
                    break;
            }
        }
    }

    public void setState(AIState state)
    {
        this.state = state;
        busy = true;
    }

    public bool isBusy()
    {
        return busy;
    }

    public bool isEnemyNearby()
    {
        return false;
    }

    public List<StationInstance> getNearbyStations()
    {
        return new List<StationInstance>();
    }

    private void fetchProduct() {

        // Siguiente producto de la receta
        currentNode = manager.currentRecipie.getLeaf();
        ProductInstance product = reachableTracker.getProductOnReach(currentNode.id);

        // Si está al alcance
        if(product != null)
        {
            // Si está en un mueble
            if (product.held)
            {
                StationInstance station = product.transform.parent.GetComponent<StationInstance>();
                if(station != null)
                {
                    goToStation(station);
                }

            }
            // Si es una herramienta
            else if (product.getProductType() == ProductType.TOOL)
            {
                ToolSource source = product.transform.parent.GetComponent<ToolSource>();
                movement.targetType = clickTargetType.TOOLSOURCE;
                movement.targetSource = source;
                nvAgent.SetDestination(source.transform.position);
                nvAgent.isStopped = false;
            }
            // Si no, está en la cinta
            else
            {
                // Vamos a coger el objeto
                movement.targetType = clickTargetType.BELT;
                movement.targetProduct = product;
                nvAgent.SetDestination(product.transform.position);
                nvAgent.isStopped = false;
            }
            currentNode.done = true;
            state = AIState.CARRY;
        }
    }

    private void carryProduct()
    {
        // Si es el parent1 (Siempre product)
        if (currentNode == currentNode.child.parent1)
        {
            //Lo llevamos a una mesa
            StationInstance station = reachableTracker.getStationOnReach(4, false);
            if (station != null)
            {
                goToStation(station);
            }
        }
        // SI es el parent2 (product o station)
        else
        {
            // Si es station
            if (currentNode.parent2.isStation)
            {
                //Lo llevamos a la estación que corresponde
                trackedStation = reachableTracker.getStationOnReach(currentNode.parent2.id, false);
                if (trackedStation != null)
                {
                    goToStation(trackedStation);
                    currentNode.parent2.done = true;
                }
            }
            //Si es product
            else
            {
                // Lo llevamos a donde estaba el parent1
                goToStation(trackedStation);
                currentNode.parent2.done = true;
            }
        }
    }

    private void goToStation(StationInstance station)
    {
        currentNode.parent2 = null;
        movement.targetType = clickTargetType.STATION;
        movement.targetStation = station;
        Vector3 dest;
        if (movement.targetStation.isReachable(transform.position))
        {
            dest = movement.targetStation.getWaitPos();
        }
        else
        {
            // CUIDADO! Puede dar problemas si se colocan de forma rara los muebles
            Vector3 dir = movement.targetStation.transform.position -  movement.targetStation.getWaitPos();
            dest = movement.targetStation.transform.position + dir;
        }
        nvAgent.SetDestination(dest);
        nvAgent.isStopped = false;

        state = AIState.FETCH;

    }
}
