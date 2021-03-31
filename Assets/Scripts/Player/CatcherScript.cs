using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatcherScript : MonoBehaviour
{

    public PlayerMovement playerMovement;

    private HashSet<StationInstance> listaTargets;
    private HashSet<ProductInstance> listaObjetos;

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
            listaObjetos.Add(other.GetComponent<ProductInstance>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Target")
            listaTargets.Remove(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
            listaObjetos.Remove(other.GetComponent<ProductInstance>());
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
                // SI lo podemos dejar en un mueble, lo dejamos
                if (station != null && station.putProduct(heldObject))
                {
                        heldObject = null;
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
                float time = station.activate();
                playerMovement.blockMovement(time, station.getWaitPos());
            }
        }
    }

    private void holdProduct(ProductInstance product)
    {
        heldObject = product;
        product.held = true;
        product.transform.SetParent(transform);
        listaObjetos.Remove(product);
    }

    private IEnumerator unblockPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        playerMovement.blocked = false;
    }
}
