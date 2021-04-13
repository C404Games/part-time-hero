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
    ProductInstance targetProduct;

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
                    if (catcher.isStationOnReach(targetStation))
                        catcher.grabBehaviour(targetStation);
                    break;
                case clickTargetType.BELT:
                    if (targetProduct != null && catcher.isProductOnReach(targetProduct))
                        catcher.holdProduct(targetProduct);
                    else
                        catcher.grabBehaviour(null);
                    break;
                case clickTargetType.TOOLSOURCE:
                    catcher.holdProduct(targetSource.heldTool.GetComponent<ProductInstance>());
                    break;
                case clickTargetType.MONSTER:
                    transform.LookAt(targetMonster.transform);
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
                        if(playerMovement.isMonsterOnReach(targetMonster))
                        {
                            nvAgent.isStopped = true;
                            nvAgent.ResetPath();
                        }
                    }
                    else
                        targetType = clickTargetType.NONE;
                    break;
                case clickTargetType.STATION:
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetStation.transform.position - transform.position), Time.deltaTime * playerMovement.rotSpeed);
                    break;
                case clickTargetType.BELT:
                    if (targetProduct != null && !targetProduct.held)
                    {
                        nvAgent.SetDestination(targetProduct.transform.position);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetProduct.transform.position - transform.position), Time.deltaTime * playerMovement.rotSpeed);
                        if (catcher.isProductOnReach(targetProduct))
                        {
                            nvAgent.isStopped = true;
                            nvAgent.ResetPath();
                        }
                    }
                    else
                    {
                        targetProduct = null;
                    }
                    break;
            }
        }
    }

    public void onMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed && !playerMovement.blocked)
        {
            // Ignorar Layer de Items
            int layerMask = LayerMask.GetMask("Item", "Player", "Catcher");

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask))
            {

                if (hit.collider.tag.Equals("Floor"))
                {
                    targetType = clickTargetType.FLOOR;
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
                        // CUIDADO! Puede dar problemas si se colocan de forma rara los muebles
                        Vector3 dir = targetStation.transform.position - targetStation.getWaitPos();
                        dest = targetStation.transform.position + dir;
                    }
                    nvAgent.SetDestination(dest);
                    nvAgent.isStopped = false;
                }
                else if (hit.collider.tag.Equals("Belt"))
                {
                    targetType = clickTargetType.BELT;

                    // Volvemos a hacer raycast pero para buscar objeto
                    layerMask = LayerMask.GetMask("Item");
                    targetProduct = null;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                    {
                        ProductInstance hitProduct = hit.collider.GetComponent<ProductInstance>();
                        if (!hitProduct.held)
                            targetProduct = hitProduct;
                    }

                    nvAgent.SetDestination(hit.point);
                    nvAgent.isStopped = false;
                }
                else if (hit.collider.tag.Equals("ToolSource"))
                {
                    targetType = clickTargetType.TOOLSOURCE;
                    targetSource = hit.collider.GetComponent<ToolSource>();
                    nvAgent.SetDestination(hit.point);
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
