using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    public static Dictionary<int, Product> productBlueprints = new Dictionary<int, Product>();
    public static Dictionary<int, Station> stationBlueprints = new Dictionary<int, Station>();

    public static Dictionary<int, Product> rawProducts = new Dictionary<int, Product>();
    public static Dictionary<int, Product> finalProducts = new Dictionary<int, Product>();

    public static Dictionary<int, Sprite> finalProductImage = new Dictionary<int, Sprite>();

    #region MonoBehaviour
    private void Awake()
    {
        string filename = "";
        switch(PlayerPrefs.GetInt("Scenary", 1))
        {
            case 1:
                filename = "RecipiesJSON/tabern";
                break;
            case 2:
                filename = "RecipiesJSON/smithy";
                break;
            case 3:
                filename = "RecipiesJSON/potions";
                break;
        }
        loadProducts(filename);
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
        rawProducts.Clear();
        finalProducts.Clear();
        finalProductImage.Clear();

        string json = Resources.Load<TextAsset>(filename).text;
        Dictionary<string, dynamic> dic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

        foreach (dynamic entry in dic["Products"])
        {
            List<Transition> transitions = new List<Transition>();
            if (null != entry["transitions"])
            {
                foreach (dynamic t in entry["transitions"])
                {
                    transitions.Add(new Transition(-1, (int)t["src"], (int)t["dst"], (int)t["time"], true));
                }
            }
            int difficulty = entry["difficulty"] != null ? entry["difficulty"] : 0;
            string appearence = (string)entry["appearence"];
            bool collapse = entry["AIcollapse"] != null;
            float time = entry["time"] != null ? (float)entry["time"] : 0;
            Product product = new Product((int)entry["id"], (string)entry["name"], (ProductType)((int)entry["type"]), difficulty, appearence, transitions, collapse, time);
            productBlueprints.Add((int)entry["id"], product);

            if(product.type == ProductType.RAW)
            {
                rawProducts.Add(product.id, product);
            }
            else if(product.type == ProductType.FINAL)
            {
                finalProducts.Add(product.id, product);
                finalProductImage.Add(product.id, Resources.Load<Sprite>((string)entry["card"]));
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
