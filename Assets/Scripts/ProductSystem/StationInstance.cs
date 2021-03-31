using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInstance : MonoBehaviour
{

    public int id;

    ProductInstance heldProduct;

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

        if (heldProduct != null) 
        {
            if (heldProduct.applyResource(product.id))
            {
                Destroy(product.gameObject);
                return true;
            }
            return false;
        }
        else
        {
            heldProduct = product;
            heldProduct.transform.parent = transform;
            heldProduct.transform.localPosition = new Vector3(0, 0, 0);
            return true;
        }

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
