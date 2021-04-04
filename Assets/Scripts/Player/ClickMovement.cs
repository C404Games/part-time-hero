using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;



enum clickTargetType
{
    STATION, 
    FLOOR,
    BELT,
    TOOLSOURCE,
    MONSTER,
    NONE
}

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(NavMeshAgent))]

public class ClickMovement : MonoBehaviour
{
    public CatcherScript catcher;

    PlayerMovement playerMovement;
    NavMeshAgent nvAgent;

    StationInstance targetStation;
    ToolSource targetSource;
    MonsterController targetMonster;

    Vector3 destination;

    clickTargetType targetType = clickTargetType.FLOOR;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        nvAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // SI hemos llegado
        if (!nvAgent.hasPath)
        {
            switch (targetType)
            {
                case clickTargetType.FLOOR:
                    break;
                case clickTargetType.STATION:
                    catcher.grabBehaviour(targetStation);
                    break;
                case clickTargetType.BELT:
                    catcher.grabBehaviour(null);
                    break;
                case clickTargetType.TOOLSOURCE:
                    catcher.holdProduct(targetSource.heldTool.GetComponent<ProductInstance>());
                    break;
                case clickTargetType.MONSTER:
                    playerMovement.attack();
                    break;
                default:
                    break;
            }
            targetType = clickTargetType.NONE;
        }
        // SI estamos de camino
        else
        {
            switch (targetType)
            {
                case clickTargetType.MONSTER:
                    if (targetMonster != null)
                    {
                        nvAgent.SetDestination(targetMonster.transform.position);
                        if(nvAgent.remainingDistance <= 2)
                        {
                            nvAgent.isStopped = true;
                            nvAgent.ResetPath();
                        }
                    }
                    else
                        targetType = clickTargetType.NONE;
                    break;
            }
        }
    }

    public void onMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed && !playerMovement.blocked)
        {
            // Ignorar Layer de Items
            int layerMask = LayerMask.GetMask("Item", "Player");
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask))
            {

                if (hit.collider.tag.Equals("Floor"))
                {
                    targetType = clickTargetType.FLOOR;
                    destination = hit.point;
                    nvAgent.SetDestination(hit.point);
                    nvAgent.isStopped = false;
                }
                else if (hit.collider.tag.Equals("Target"))
                {
                    targetType = clickTargetType.STATION;
                    targetStation = hit.collider.GetComponent<StationInstance>();
                    Vector3 dest;
                    if (targetStation.isReachable(transform.position))
                    {
                        dest = targetStation.getWaitPos();
                    }
                    else
                    {
                        Vector3 dir = targetStation.transform.position - targetStation.getWaitPos();
                        dest = targetStation.transform.position + dir;
                    }
                    nvAgent.SetDestination(dest);
                    destination = targetStation.transform.position;
                    nvAgent.isStopped = false;
                }
                else if (hit.collider.tag.Equals("Belt"))
                {
                    targetType = clickTargetType.BELT;
                    destination = hit.point;
                    nvAgent.SetDestination(destination);
                    nvAgent.isStopped = false;
                }
                else if (hit.collider.tag.Equals("ToolSource"))
                {
                    targetType = clickTargetType.TOOLSOURCE;
                    targetSource = hit.collider.GetComponent<ToolSource>();
                    destination = hit.point;
                    nvAgent.SetDestination(destination);
                    nvAgent.isStopped = false;
                }
                else if (hit.collider.tag.Equals("Monster"))
                {
                    targetType = clickTargetType.MONSTER;
                    targetMonster = hit.collider.GetComponent<MonsterController>();
                    nvAgent.SetDestination(targetMonster.transform.position);
                    nvAgent.isStopped = false;
                }
            }
        }

    }
}
