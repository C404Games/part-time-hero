using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatcherScript : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;
    public ReachableTracker reachableTracker;

    private HashSet<StationInstance> listaTargets;
    private HashSet<ProductInstance> listaObjetos;

    private DeliverySpot deliverySpot;

    private ProductInstance heldObject;

    private void Start()
    {
        listaObjetos = new HashSet<ProductInstance>();
        listaTargets = new HashSet<StationInstance>();
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
                    playerMovement.blockMovement(time, station.getWaitPos(), station.getWaitRot());
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
            if (deliverySpot != null && deliverySpot.deliverProduct(heldObject))
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
                    playerMovement.blockMovement(time, station.getWaitPos(), station.getWaitRot());
                heldObject.held = false;
                heldObject = null;
                animator.SetBool("Hold", false);
            }
        }
    }

    public void holdProduct(ProductInstance product)
    {
        if (heldObject == null && product != null && !product.held)
        {
            animator.SetBool("Hold", true);
            heldObject = product;
            product.held = true;
            product.transform.SetParent(transform);
            product.GetComponent<Rigidbody>().isKinematic = true;
            listaObjetos.Remove(product);
        }
    }

}
