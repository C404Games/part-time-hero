﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatcherScript : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;
    public ReachableTracker reachableTracker;

    private DeliverySpot deliverySpot;

    private ProductInstance heldObject;

    private void Start()
    {
    }

    public void OnGrab(InputAction.CallbackContext context)
    {

        if (context.performed) { 
            grabBehaviour(reachableTracker.getNearestStation());
        }
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StationInstance station = reachableTracker.getNearestStation();
            if (station != null)
            {
                float time = station.activate(transform.parent.position);
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
            if (station != null && !station.isBusy())
            {
                ProductInstance p = station.takeProduct();
                if (p != null)
                {
                    holdProduct(p);
                    return;
                }
            }
            //Cogemos el objeto libre cercano si hay
            ProductInstance product = reachableTracker.getNearestProduct();
            holdProduct(product);
        }

        // SI llevamos algo
        else
        {
            // SI lo podemos dejar en un punto de entrega, lo dejamos
            if (reachableTracker.getDeliverySpotOnReach() != null && reachableTracker.getDeliverySpotOnReach().deliverProduct(heldObject))
            {
                animator.SetBool("Hold", false);
            }
            // SI no... Si lo podemos dejar en un mueble, lo dejamos
            else if (station != null)
            {
                float time = station.putProduct(heldObject, transform.parent.position);
                if (time < 0)
                    return;
                if (time > 0)
                    playerMovement.blockMovement(time, station.getWaitPos(playerMovement.transform.position), station.getWaitRot(playerMovement.transform.position));
                heldObject = null;
                animator.SetBool("Hold", false);
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
            product.GetComponent<Rigidbody>().isKinematic = true;
            product.GetComponent<Rigidbody>().useGravity= false;
            //product.GetComponent<BoxCollider>().isTrigger = true;
        }
    }

}
