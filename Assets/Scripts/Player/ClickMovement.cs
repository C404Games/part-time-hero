using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Photon.Pun;

public enum clickTargetType
{
    STATION, 
    FLOOR,
    BELT,
    TOOLSOURCE,
    DELIVERY,
    BOOK,
    MONSTER,
    NONE
}

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(NavMeshAgent))]

public class ClickMovement : MonoBehaviour
{
    public CatcherScript catcher;
    public ReachableTracker reachableTracker;

    public bool guaranteeTargetProduct = false;

    PlayerMovement playerMovement;
    NavMeshAgent nvAgent;

    [HideInInspector] public StationInstance targetStation;
    [HideInInspector] public DeliverySpot targetDeliverySpot;
    [HideInInspector] public ToolSource targetSource;
    [HideInInspector] public MonsterController targetMonster;
    [HideInInspector] public ProductInstance targetProduct;
    [HideInInspector] public RecipieBook targetBook;

    [HideInInspector] public clickTargetType targetType = clickTargetType.FLOOR;

    public bool active = true;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

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
                    if (reachableTracker.isStationOnReach(targetStation))
                        catcher.grabBehaviour(targetStation);
                    break;
                case clickTargetType.BELT:
                    if (targetProduct != null && reachableTracker.isProductOnReach(targetProduct))
                        catcher.holdProduct(targetProduct);
                    else if(!guaranteeTargetProduct)
                        catcher.grabBehaviour(null);
                    break;
                case clickTargetType.TOOLSOURCE:
                    if (catcher.getHeldProduct() == null)
                        catcher.holdProduct(targetSource.heldTool.GetComponent<ProductInstance>());
                    else if (catcher.getHeldProduct().id == targetSource.toolId)
                    {
                        targetSource.returnTool(catcher.getHeldProduct());
                        catcher.releaseHeldProduct();
                    }
                    break;
                case clickTargetType.DELIVERY:
                    catcher.grabBehaviour(null);
                    break;
                case clickTargetType.BOOK:
                    playerMovement.openCloseBook();
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
                case clickTargetType.DELIVERY:
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDeliverySpot.transform.position - transform.position), Time.deltaTime * playerMovement.rotSpeed);
                    if(reachableTracker.getDeliverySpotOnReach() != null)
                    {
                        nvAgent.isStopped = true;
                        nvAgent.ResetPath();
                    }
                    break;
                case clickTargetType.BOOK:
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetBook.transform.position - transform.position), Time.deltaTime * playerMovement.rotSpeed);
                    if (reachableTracker.getReachableBook() != null)
                    {
                        nvAgent.isStopped = true;
                        nvAgent.ResetPath();
                    }
                    break;
                case clickTargetType.BELT:
                    if (targetProduct != null && !targetProduct.isHeld())
                    {
                        nvAgent.SetDestination(targetProduct.transform.position);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetProduct.transform.position - transform.position), Time.deltaTime * playerMovement.rotSpeed);
                        if (reachableTracker.isProductOnReach(targetProduct))
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

    public bool isBlocked()
    {
        return playerMovement.blocked;
    }

    public bool isFrozen()
    {
        return playerMovement.frozen;
    }

    public void stop()
    {
        nvAgent.isStopped = true;
    }

    public void resume()
    {
        nvAgent.isStopped = false;
    }

    public void onMouseClick(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (active && context.performed)
        {
            if (!playerMovement.blocked && !playerMovement.frozen)
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
                        dest = targetStation.getWaitPos(transform.position);
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
                            if (!hitProduct.isHeld())
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
                    else if (hit.collider.tag.Equals("RecipieBook"))
                    {
                        targetType = clickTargetType.BOOK;
                        targetBook = hit.collider.GetComponent<RecipieBook>();
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
                    else if (hit.collider.tag.Equals("Delivery"))
                    {
                        targetType = clickTargetType.DELIVERY;
                        targetDeliverySpot = hit.collider.GetComponent<DeliverySpot>();
                        Vector3 dest = targetDeliverySpot.transform.position;
                        nvAgent.SetDestination(dest);
                        nvAgent.isStopped = false;
                    }
                }
            }
            else if (playerMovement.isBookOpen())
            {
                playerMovement.openCloseBook();
            }
        }
    }
}
