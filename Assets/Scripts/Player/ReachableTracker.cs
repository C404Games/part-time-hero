using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReachableTracker : MonoBehaviour
{
    public GameObject player;

    List<ProductInstance> reachableProducts;
    List<StationInstance> reachableStations;
    List<MonsterController> reachableMonsters;
    //List<ToolSource> reachableToolSources;
    RecipieBook reachableBook;
    DeliverySpot deliverySpot;

    // Start is called before the first frame update
    void Awake()
    {
        reachableProducts = new List<ProductInstance>();
        reachableStations = new List<StationInstance>();
        reachableMonsters = new List<MonsterController>();
        //reachableToolSources = new List<ToolSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
            reachableStations.Add(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
        {
            ProductInstance product = other.GetComponent<ProductInstance>();
            //if (!product.isHeld())
            reachableProducts.Add(product);
        }
        //else if(other.tag == "ToolSource")
        //    reachableToolSources.Add(other.GetComponent<ToolSource>());
        else if (other.tag == "Delivery")
            deliverySpot = other.GetComponent<DeliverySpot>();
        else if (other.tag == "RecipieBook")
            reachableBook = other.GetComponent<RecipieBook>();
        else if (other.tag == "Monster")
            reachableMonsters.Add(other.GetComponent<MonsterController>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Target")
            reachableStations.Remove(other.GetComponent<StationInstance>());
        else if (other.tag == "Item")
        {
            ProductInstance product = other.GetComponent<ProductInstance>();
            reachableProducts.Remove(product);
        }
        else if (other.tag == "RecipieBook")
            reachableBook = null;
        else if (other.tag == "Delivery")
            deliverySpot = null;
        else if (other.tag == "Monster")
            reachableMonsters.Remove(other.GetComponent<MonsterController>());
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
            if (station.id == id && station.getHealth() > 0 && station.isOccupied() == occupied &&
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
            if (station.id == id && station.getHealth() > 0 && station.isOccupied() == occupied)
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
            if (station.name == name && station.isOccupied() == occupied && station.getHealth() > 0 &&
                Vector3.Distance(station.transform.position, player.transform.position) < minDist)
                minDistStation = station;
        }
        return minDistStation;
    }

    public StationInstance getRandomStation()
    {
        List<StationInstance> stations = reachableStations.Where(s => s.getHealth() > 0).ToList();
        if (stations.Count == 0)
            return null;
        return stations[Random.Range(0, stations.Count)];
    }

    public StationInstance getRandomBreakableStation()
    {
        List<StationInstance> stations = reachableStations.Where(s => s.getHealth() > 0 && s.isBreakable()).ToList();
        if (stations.Count == 0)
            return null;
        return stations[Random.Range(0, stations.Count)];
    }

    public StationInstance getNearbyBrokenStation()
    {
        float minDist = float.MaxValue;
        StationInstance minDistStation = null;
        foreach (StationInstance station in reachableStations)
        {
            if (station.getHealth() <= 0 && !station.isBusy() && Vector3.Distance(station.transform.position, player.transform.position) < minDist)
                minDistStation = station;
        }
        return minDistStation;
    }

    public DeliverySpot getDeliverySpotOnReach()
    {
        return deliverySpot;
    }

    public RecipieBook getReachableBook()
    {
        return reachableBook;
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

    public MonsterController getNearestMonster()
    {
        reachableMonsters.RemoveAll(item => item == null);
        double minDistance = Mathf.Infinity;
        MonsterController minDistanceMonster = null;

        foreach (MonsterController monster in reachableMonsters)
        {
            if (Vector3.Distance(monster.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(monster.transform.position, transform.position);
                minDistanceMonster = monster;
            }
        }

        return minDistanceMonster;
    }
}
