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

    public RecipieNode currentNode;

    ProductInstance targetProduct;

    //StationInstance targetStation;

    ToolSource targetToolSource;

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
    }

    public void setState(AIState state)
    {
        this.state = state;
        busy = true;
    }

    public bool isBusy()
    {
        return movement.targetType != clickTargetType.NONE;
    }   

    // Para el Behaviour Tree necesitamos funciones de acción para los nodos hoja que hacen cosas 
    // Otras funciones para los nodos hoja que "preguntan" cosas 

    // Hay dos Berhaviour Trees, uno para cuando buscamos un objeto (fetch) y otro para cuando queremos dejar el objeto (carry)

    ////////////////////////
    ////// BT ACTIONS///////
    ////////////////////////
    #region fetch
    public bool updateTarget()
    {
        // Siguiente producto de la receta
        currentNode = manager.currentRecipie.getLeaf();
        targetProduct = reachableTracker.getProductOnReach(currentNode.id);
        if(targetProduct != null)
            return true;
        return false;
    }
    public bool goToTargetStation()
    {
        //Lo llevamos a la estación que corresponde
        StationInstance station = targetProduct.holder.GetComponent<StationInstance>();
        if (station != null && !station.isBusy())
        {
            goToStation(station);
            currentNode.done = true;
            state = AIState.CARRY;
        }
       
        return true;
    }
    public bool goToToolSource()
    {
        ToolSource source = targetProduct.holder.GetComponent<ToolSource>();
        movement.targetType = clickTargetType.TOOLSOURCE;
        movement.targetSource = source;
        nvAgent.SetDestination(source.transform.position);
        nvAgent.isStopped = false;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, nvAgent.destination, NavMesh.AllAreas, path);
        nvAgent.SetPath(path);

        currentNode.done = true;
        state = AIState.CARRY;

        return true;
    }
    public bool goToBelt()
    {
        // Vamos a coger el objeto
        movement.targetType = clickTargetType.BELT;
        movement.targetProduct = targetProduct;
        nvAgent.SetDestination(targetProduct.transform.position);
        nvAgent.isStopped = false;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, nvAgent.destination, NavMesh.AllAreas, path);
        nvAgent.SetPath(path);

        currentNode.done = true;
        state = AIState.CARRY;

        return true;
    }
    #endregion

    #region carry
    public bool carryToTable()
    {
        //Lo llevamos a una mesa
        trackedStation = reachableTracker.getStationOnReach(4, false);
        if (trackedStation != null)
        {
            goToStation(trackedStation);
        }
        state = AIState.FETCH;
        return true;
    }

    public bool carryToTracked()
    {
        // Lo llevamos a donde estaba el parent1
        goToStation(trackedStation);
        currentNode.done = true;
        state = AIState.FETCH;
        return true;
    }

    public bool carryToPartnerStation()
    {
        //Lo llevamos a la estación que corresponde
        trackedStation = reachableTracker.getStationOnReach(currentNode.child.parent2.id, false);
        if (trackedStation != null)
        {
            goToStation(trackedStation);
            currentNode.done = true;
            currentNode.child.parent2.done = true;
            state = AIState.FETCH;
        }
        return true;
    }

    public bool deliverProduct()
    {
        // No de eso aún jeje
        return false;
    }

    public bool nextRecipie()
    {
        manager.nextRecipie();
        return true;
    }
    #endregion


    ////////////////////////
    ////// BT QUERIES///////
    ////////////////////////
    #region fetchQuery
    public bool isTargetInReach()
    {
        return targetProduct != null;
    }
    public bool isTargetHeld()
    {
        return targetProduct.isHeld();
    }
    public bool isTargetTool()
    {
        return targetProduct.getProductType() == ProductType.TOOL;
    }
    #endregion

    #region carryQuery
    public bool isParent1()
    {
        return currentNode == currentNode.child.parent1;
    }

    public bool isJoinedWithProduct()
    {
        // Si el currentNode va con otro product
        return !currentNode.child.parent2.isStation;
    }

    public bool isNodeStation()
    {
        return currentNode.isStation;
    }

    public bool isFinalProduct()
    {
        return targetProduct.getProductType() == ProductType.FINAL;
    }
    #endregion

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
            Vector3 dir = movement.targetStation.transform.position - movement.targetStation.getWaitPos();
            dest = movement.targetStation.transform.position + dir;
        }
        nvAgent.SetDestination(dest);
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, nvAgent.destination, NavMesh.AllAreas, path);
        nvAgent.SetPath(path);
        nvAgent.isStopped = false;

        state = AIState.FETCH;

    }
}
