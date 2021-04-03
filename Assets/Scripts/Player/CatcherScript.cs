using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatcherScript : MonoBehaviour
{

    public PlayerMovement playerMovement;
    public Animator animator;

    private HashSet<StationInstance> listaTargets;
    private HashSet<ProductInstance> listaObjetos;

    private DeliverySpot deliverySpot;

    private ProductInstance heldObject;

    public ProductInstance objetoMasCercano()
    {
        double minDistance = Mathf.Infinity;
        ProductInstance minDistanceObject = null;

        foreach (ProductInstance product in listaObjetos)
        {
            if(Vector3.Distance(product.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(product.transform.position, transform.position);
                minDistanceObject = product;
            }
        }

        return minDistanceObject;
    }

    public StationInstance targetMasCercano()
    {
        double minDistance = Mathf.Infinity;
        StationInstance minDistanceTarget = null;

        foreach (StationInstance station in listaTargets)
        {
            if (Vector3.Distance(station.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(station.transform.position, transform.position);
                minDistanceTarget = station;
            }
        }

        return minDistanceTarget;
    }

    public void eliminarObjeto(ProductInstance product)
    {
        listaObjetos.Remove(product);
    }

    private void Start()
    {
        listaObjetos = new HashSet<ProductInstance>();
        listaTargets = new HashSet<StationInstance>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
            listaTargets.Add(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
        {
            ProductInstance product = other.GetComponent<ProductInstance>();
            if (!product.held)
                listaObjetos.Add(product);
        }
        else if (other.tag == "Delivery")
            deliverySpot = other.GetComponent<DeliverySpot>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Target")
            listaTargets.Remove(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
            listaObjetos.Remove(other.GetComponent<ProductInstance>());
        else if (other.tag == "Delivery")
            deliverySpot = null;
            
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        StationInstance station = targetMasCercano();

        if (context.performed)
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
                ProductInstance product = objetoMasCercano();
                if (product != null && !product.held)
                {
                    holdProduct(product);
                }
            }

            // SI llevamos algo
            else
            {
                // SI lo podemos dejar en un punto de entrega, lo dejamos
                if (deliverySpot != null && deliverySpot.deliverProduct(heldObject))
                {
                    animator.SetBool("Holding", false);
                }
                // SI no... Si lo podemos dejar en un mueble, lo dejamos
                else if (station != null)
                {
                    float time = station.putProduct(heldObject, transform.parent.position);
                    if (time < 0)
                        return;
                    if (time > 0)
                        playerMovement.blockMovement(time, station.getWaitPos());
                    heldObject = null;
                    animator.SetBool("Holding", false);
                }
            }
        }
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StationInstance station = targetMasCercano();
            if (station != null)
            {
                float time = station.activate(transform.parent.position);
                if(time > 0)
                    playerMovement.blockMovement(time, station.getWaitPos());
            }
        }
    }

    private void holdProduct(ProductInstance product)
    {
        animator.SetBool("Holding", true);
        heldObject = product;
        product.held = true;
        product.transform.SetParent(transform);
        product.GetComponent<Rigidbody>().isKinematic = true;
        listaObjetos.Remove(product);
    }

}
