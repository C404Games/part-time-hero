using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    List<RecipieNode> recipies;

    public RecipieNode currentRecipie;

    public AIAgent[] agents;

    // Start is called before the first frame update
    void Start()
    {
        recipies = new List<RecipieNode>();
        genProductTrees();
        currentRecipie = recipies[0].copySelf(null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void genProductTrees()
    {
        foreach(Product product in ProductManager.finalProducts)
        {
            recipies.Add(createProdNode(product.id, null));
        }
    }

    private RecipieNode createProdNode(int id, RecipieNode child)
    {
        RecipieNode node = new RecipieNode(id);
        node.isStation = false;
        node.child = child;

        // Miramos si viene de un product
        foreach(Product product in ProductManager.productBlueprints.Values)
        {
            foreach(Transition transition in product.transitions)
            {
                if(transition.dst == id)
                {
                    // Encontrado padre
                    if(node.parent1 == null)
                    {
                        node.parent1 = createProdNode(product.id, node);
                    }
                    else if (node.parent2 == null)
                    {
                        node.parent2 = createProdNode(product.id, node);
                    }
                }
            }
        }
        // Si no viene de un product, viene de un station
        if(node.parent1 == null)
        {
            foreach(Station station in ProductManager.stationBlueprints.Values)
            {
                foreach(Transition transition in station.transitions)
                {
                    // Encontrado padre
                    if(transition.dst == id)
                    {
                        node.parent1 = createProdNode(transition.src, node);
                        node.parent2 = createStatNode(station.id, node);
                    }
                }
            }
        }

        return node;
    }

    private RecipieNode createStatNode(int id, RecipieNode child)
    {
        RecipieNode node = new RecipieNode(id);
        node.isStation = true;
        node.child = child;
        return node;
    }

}
