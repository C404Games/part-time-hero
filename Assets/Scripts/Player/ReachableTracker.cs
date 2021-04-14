using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachableTracker : MonoBehaviour
{

    List<ProductInstance> reachableProducts;
    List<StationInstance> reachableStations;
    List<ToolSource> reachableToolSources;
    DeliverySpot deliverySpot;

    // Start is called before the first frame update
    void Start()
    {
        reachableProducts = new List<ProductInstance>();
        reachableStations = new List<StationInstance>();
        reachableToolSources = new List<ToolSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
            reachableStations.Add(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
        {
            ProductInstance product = other.GetComponent<ProductInstance>();
            if (!product.held)
                reachableProducts.Add(product);
        }
        else if(other.tag == "ToolSource")
        {
            reachableToolSources.Add(other.GetComponent<ToolSource>());
        }
        else if (other.tag == "Delivery")
            deliverySpot = other.GetComponent<DeliverySpot>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Target")
            reachableStations.Remove(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
            reachableProducts.Remove(other.GetComponent<ProductInstance>());
        else if (other.tag == "Delivery")
            deliverySpot = null;

    }

    public ProductInstance getProductOnReach(int id)
    {
        foreach (ProductInstance product in reachableProducts)
        {
            if (product.id == id)
                return product;
        }
        return null;
    }

    public StationInstance getStationOnReach(int id, bool occupied)
    {
        foreach (StationInstance station in reachableStations)
        {
            if (station.id == id && station.isOccupied() == occupied)
                return station;
        }
        return null;
    }

    public StationInstance getStationOnReach(string name, bool occupied)
    {
        foreach (StationInstance station in reachableStations)
        {
            if (station.name == name && station.isOccupied() == occupied)
                return station;
        }
        return null;
    }

    public bool isProductOnReach(ProductInstance product)
    {
        return reachableProducts.Contains(product);
    }

    public bool isStationOnReach(StationInstance station)
    {
        return reachableStations.Contains(station);
    }

    public ProductInstance getNearestProduct()
    {
        double minDistance = Mathf.Infinity;
        ProductInstance minDistanceObject = null;

        foreach (ProductInstance product in reachableProducts)
        {
            if (Vector3.Distance(product.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(product.transform.position, transform.position);
                minDistanceObject = product;
            }
        }

        return minDistanceObject;
    }

    public StationInstance getNearestStation()
    {
        double minDistance = Mathf.Infinity;
        StationInstance minDistanceTarget = null;

        foreach (StationInstance station in reachableStations)
        {
            if (Vector3.Distance(station.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(station.transform.position, transform.position);
                minDistanceTarget = station;
            }
        }

        return minDistanceTarget;
    }
}
