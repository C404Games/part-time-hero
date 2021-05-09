using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    public static Dictionary<int, Product> productBlueprints = new Dictionary<int, Product>();
    public static Dictionary<int, Station> stationBlueprints = new Dictionary<int, Station>();

    public static Dictionary<int, Product> rawProducts = new Dictionary<int, Product>();
    public static Dictionary<int, Product> finalProducts = new Dictionary<int, Product>();

    public List<Product> nonStaticRawProducts;
    public List<Product> nonStaticFinalProducts;

    #region MonoBehaviour
    private void Awake()
    {      
        nonStaticFinalProducts = new List<Product>();
        nonStaticRawProducts = new List<Product>();
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
                    transitions.Add(new Transition((int)t["src"], (int)t["dst"], (int)t["time"]));
                }
            }
            int difficulty = entry["difficulty"] != null ? entry["difficulty"] : 0;
            string appearence = (string)entry["appearence"];
            bool collapse = entry["AIcollapse"] != null;
            Product product = new Product((int)entry["id"], (string)entry["name"], (ProductType)((int)entry["type"]), difficulty, appearence, transitions, collapse);
            productBlueprints.Add((int)entry["id"], product);

            if(product.type == ProductType.RAW)
            {
                nonStaticRawProducts.Add(product);
                rawProducts.Add(product.id, product);
            }
            else if(product.type == ProductType.FINAL)
            {
                nonStaticFinalProducts.Add(product);
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
                    transitions.Add(new Transition((int)t["src"], (int)t["dst"], (int)t["time"]));
                }
            }
            stationBlueprints.Add((int)entry["id"], new Station((int)entry["id"], (string)entry["name"], (bool)entry["auto"], transitions));
        }
    }
}
