﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInstance : MonoBehaviour
{

    public int id;

    public Transform waitPosition;

    ProductInstance heldProduct;

    Station blueprint;

    int health;

    RadialClockController clockController;

    // Start is called before the first frame update
    void Start()
    {
        clockController = FindObjectOfType<RadialClockController>();
        blueprint = ProductManager.stationBlueprints[id];
        health = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getWaitPos()
    {
        return waitPosition.position;
    }

    // Devuelve el tiempo en segundos si NO es auto. Devuelve 0 si es auto.
    public float putProduct(ProductInstance product)
    {

        if (heldProduct != null) 
        {
            if (heldProduct.applyResource(product.id))
            {
                Destroy(product.gameObject);
                return 0;
            }
            return -1;
        }
        else
        {
            heldProduct = product;
            heldProduct.transform.parent = transform;
            return activate();
        }

    }

    // Devuelve el tiempo en segundos si NO es auto. Devuelve 0 si es auto.
    public float activate()
    {
        if(heldProduct != null)
        {
            foreach(Transition transition in blueprint.transitions)
            {
                if(heldProduct.id == transition.src)
                {
                    StartCoroutine(heldProduct.transformProduct(transition.dst, transition.time));

                    if(transition.time > 0)
                        clockController.startClock(this, transition.time);

                    return blueprint.auto ? 0 : transition.time;
                }
            }
        }
        return 0;
    }

    public ProductInstance takeProduct()
    {
        ProductInstance product = heldProduct;
        heldProduct = null;
        return product;
    }

    public void takeHealth(int h)
    {
        health -= h;
        if(health <= 0)
        {
            // Cambiar apariencia
        }
    }

    public int getHealth()
    {
        return health;
    }

}
