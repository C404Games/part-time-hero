using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatcherScript : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public ReachableTracker reachableTracker;

    private Animator animator;
    private DeliverySpot deliverySpot;

    private ProductInstance heldObject;

    public bool active = true;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = playerMovement.GetComponent<PhotonView>();
    }

    private void Start()
    {
        animator = transform.parent.GetComponentInChildren<Animator>();
    }

    public ProductInstance getHeldProduct()
    {
        return heldObject;
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (!PhotonNetwork.OfflineMode && !photonView.IsMine)
        {
            return;
        }
        if (active && context.performed) { 
            grabBehaviour(reachableTracker.getNearestStation());
        }
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (active && context.performed)
        {
            StationInstance station = reachableTracker.getNearestStation();
            if (station != null)
            {
                float time = station.activate();
                if(time > 0)
                    playerMovement.blockMovement(time, station.getWaitPos(playerMovement.transform.position), station.getWaitRot(playerMovement.transform.position));
            }
        }
    }

    public void grabBehaviour(StationInstance station)
    {

        // SI no llevamos nada
        if (heldObject == null)
        {
            //Cogemos el objeto del target si hay
            if (station != null)
            {
                float time = station.interact(this);
                if(time > 0)
                    playerMovement.blockMovement(time, station.getWaitPos(playerMovement.transform.position), station.getWaitRot(playerMovement.transform.position));
            }
            //Cogemos el objeto libre cercano si hay
            else
            {
                ProductInstance product = reachableTracker.getNearestProduct();
                holdProduct(product);
            }
        }

        // SI llevamos algo
        else
        {
            // SI lo podemos dejar en un punto de entrega, lo dejamos
            if (reachableTracker.getDeliverySpotOnReach() != null)
            {
                reachableTracker.getDeliverySpotOnReach().deliverProduct(playerMovement.team, heldObject);
                animator.SetBool("Hold", false);
            }
            // SI no... Si lo podemos dejar en un mueble, lo dejamos
            else if (station != null)
            {
                float time = station.putProduct(heldObject, this);
                if (time > 0)
                    playerMovement.blockMovement(time, station.getWaitPos(playerMovement.transform.position), station.getWaitRot(playerMovement.transform.position));
            }
        }
    }

    public void holdProduct(ProductInstance product)
    {
        if (heldObject == null && product != null)// && !product.isHeld())
        {
            animator.SetBool("Hold", true);
            heldObject = product;
            //product.transform.SetParent(transform);
            product.setHolder(transform);            
            //product.GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    public void releaseHeldProduct()
    {
        if (heldObject.getHolder() == transform)
            heldObject.setHolder(null);
        heldObject = null;
        animator.SetBool("Hold", false);
    }

}
