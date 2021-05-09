using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    public static Dictionary<int, Product> productBlueprints = new Dictionary<int, Product>();
    public static Dictionary<int, Station> stationBlueprints = new Dictionary<int, Station>();

    public static Dictionary<int, Product> rawProducts = new Dictionary<int, Product>();
    public static Dictionary<int, Product> finalProducts = new Dictionary<int, Product>();

    #region MonoBehaviour
    private void Awake()
    {      
        loadProducts("products");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    public void loadProducts(string filename)
    {
        productBlueprints.Clear();
        stationBlueprints.Clear();


        string json = Resources.Load<TextAsset>(filename).text;
        Dictionary<string, dynamic> dic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

        foreach (dynamic entry in dic["Products"])
        {
            List<Transition> transitions = new List<Transition>();
            if (entry["transitions"] != null)
            {
                foreach (dynamic t in entry["transitions"])
                {
                    transitions.Add(new Transition(-1, (int)t["src"], (int)t["dst"], (int)t["time"], true));
                }
            }
            int difficulty = entry["difficulty"] != null ? entry["difficulty"] : 0;
            string appearence = (string)entry["appearence"];
            bool collapse = entry["AIcollapse"] != null;
            Product product = new Product((int)entry["id"], (string)entry["name"], (ProductType)((int)entry["type"]), difficulty, appearence, transitions, collapse);
            productBlueprints.Add((int)entry["id"], product);

            if(product.type == ProductType.RAW)
            {
                rawProducts.Add(product.id, product);
            }
            else if(product.type == ProductType.FINAL)
            {
                finalProducts.Add(product.id, product);
            }

        }

        foreach(dynamic entry in dic["Stations"])
        {
            List<Transition> transitions = new List<Transition>();
            if (entry["transitions"] != null)
            {
                foreach (dynamic t in entry["transitions"])
                {
                    int pre = t["pre"] != null ? (int)t["pre"] : -1;
                    transitions.Add(new Transition(pre, (int)t["src"], (int)t["dst"], (int)t["time"], (bool)t["auto"]));
                }
            }

            List<int> noHold = new List<int>();

            if(entry["no_hold"] != null)
            {
                foreach(dynamic p in entry["no_hold"])
                {
                    noHold.Add((int)p);
                }
            }

            bool breakable = entry["breakable"] != null ? (bool)entry["breakable"] : false;

            stationBlueprints.Add((int)entry["id"], new Station((int)entry["id"], (string)entry["name"], transitions, noHold, breakable));
        }
    }
}
