using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachableTracker : MonoBehaviour
{
    public GameObject player;

    List<ProductInstance> reachableProducts;
    List<StationInstance> reachableStations;
    //List<ToolSource> reachableToolSources;
    DeliverySpot deliverySpot;

    // Start is called before the first frame update
    void Awake()
    {
        reachableProducts = new List<ProductInstance>();
        reachableStations = new List<StationInstance>();
        //reachableToolSources = new List<ToolSource>();
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
            if (!product.isHeld())
                reachableProducts.Add(product);
        }
        //else if(other.tag == "ToolSource")
        //    reachableToolSources.Add(other.GetComponent<ToolSource>());
        else if (other.tag == "Delivery")
            deliverySpot = other.GetComponent<DeliverySpot>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Target")
            reachableStations.Remove(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
            reachableProducts.Remove(other.GetComponent<ProductInstance>());
        //else if (other.tag == "ToolSource")
        //    reachableToolSources.Remove(other.GetComponent<ToolSource>());
        else if (other.tag == "Delivery")
            deliverySpot = null;

    }

    public ProductInstance getProductOnReach(int id)
    {
        reachableProducts.RemoveAll(item => item == null);
        float minDist = float.MaxValue;
        ProductInstance minDistProduct = null;
        foreach (ProductInstance product in reachableProducts)
        {
            if (product.id == id &&
                Vector3.Distance(product.transform.position, player.transform.position) < minDist)
                minDistProduct = product;
        }
        return minDistProduct;
    }

    public StationInstance getStationOnReach(int id, bool occupied)
    {
        float minDist = float.MaxValue;
        StationInstance minDistStation = null;
        foreach (StationInstance station in reachableStations)
        {
            if (station.id == id && station.isOccupied() == occupied &&
                Vector3.Distance(station.transform.position, player.transform.position) < minDist)
            {
                minDistStation = station;
            }
        }
        return minDistStation;
    }

    public List<StationInstance> getStationListOnReach(int id, bool occupied)
    {
        List<StationInstance> stationList = new List<StationInstance>();
        foreach (StationInstance station in reachableStations)
        {
            if (station.id == id && station.isOccupied() == occupied)
            {
                stationList.Add(station);
            }
        }
        return stationList;
    }

    public StationInstance getStationOnReach(string name, bool occupied)
    {
        float minDist = float.MaxValue;
        StationInstance minDistStation = null;
        foreach (StationInstance station in reachableStations)
        {
            if (station.name == name && station.isOccupied() == occupied &&
                Vector3.Distance(station.transform.position, player.transform.position) < minDist)
                minDistStation = station;
        }
        return minDistStation;
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
        reachableProducts.RemoveAll(item => item == null);
        double minDistance = Mathf.Infinity;
        ProductInstance minDistanceObject = null;

        foreach (ProductInstance product in reachableProducts)
        {
            if (!product.isHeld() && Vector3.Distance(product.transform.position, transform.position) < minDistance)
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
