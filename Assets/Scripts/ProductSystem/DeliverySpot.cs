﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour
{

    MatchManager matchManager;

    // Start is called before the first frame update
    void Start()
    {
        matchManager = FindObjectOfType<MatchManager>();
    }

    public bool deliverProduct(int team, ProductInstance product)
    {
        if(product.getProductType() == ProductType.FINAL && matchManager.deliverProduct(team, product.id))
        {
            // Avisar al manager que lleve eso
            Destroy(product.gameObject);
            return true;
        }
        return false;
    }
}
