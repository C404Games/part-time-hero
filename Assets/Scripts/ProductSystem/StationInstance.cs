using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInstance : MonoBehaviour
{

    public int id;

    ProductInstance holding;

    Station blueprint;

    int health;

    // Start is called before the first frame update
    void Start()
    {
        blueprint = ProductManager.stationBlueprints[id];
        health = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool putProduct(ProductInstance product)
    {
        if (holding)
        {
            foreach(Transition t in blueprint.transitions)
            {
                if(t.src == product.id)
                {
                    StartCoroutine(holding.transformProduct(t.src, t.time));
                    return true;
                }
            }
            return false;
        }
        holding = product;
        holding.transform.parent = transform;
        holding.transform.localPosition = new Vector3(0, 0, 0);
        return true;
    }

    public ProductInstance takeProduct()
    {
        return holding;
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
