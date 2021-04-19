﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum AIStep
{
    STEP1,
    STEP2,
    STEP3
}

public class AIManager : MonoBehaviour
{

    List<RecipieNode> recipies;

    public RecipieNode currentRecipie;

    public RecipieNode currentNode;

    public AIAgent[] agents;

    public AIAgent activeAgent;

    StationInstance commonTable;
    ProductInstance commonProduct;

    AIStep step;

    bool productJoin = false;


    // Start is called before the first frame update
    void Start()
    {
        recipies = new List<RecipieNode>();
        genProductTrees();
        // Cojemos una receta cualquiera
        nextRecipie();

        int id = 0;
        foreach (AIAgent agent in agents)
        {
            agent.id = id++;
        }

        activeAgent = agents[0];

        step = AIStep.STEP1;
    }

    // Update is called once per frame
    void Update()
    {
        // Si el activeAgent ha terminado
        if (activeAgent == null || !activeAgent.busy)
        {

            switch (step)
            {
                case AIStep.STEP1:
                    {
                        // Sacamos siguiente nodo
                        currentNode = currentRecipie.getLeaf();

                        // SI es null, hemos terminado la receta
                        if(currentNode == null)
                        {
                            nextRecipie();
                            step = AIStep.STEP3;
                            break;
                        }

                        // El parent 1 del nodo está garantizado que sea Producto
                        activeAgent = null;
                        int i;
                        for (i = 0; i < agents.Length; i++)
                        {
                            // Si el agente 'i' tene el producto al alcance, pasa a ser el activeAgent
                            ProductInstance product = agents[i].reachableTracker.getProductOnReach(currentNode.parent1.id);
                            if (product != null)
                            {
                                activeAgent = agents[i];
                                activeAgent.targetProduct = product;
                                break;
                            }
                        }
                        if (activeAgent == null)
                            break;

                        commonProduct = activeAgent.targetProduct;

                        // Situación producto-mueble
                        if (currentNode.parent2.isStation)
                        {
                            StationInstance station = agents[i].reachableTracker.getStationOnReach(currentNode.parent2.id, false);
                            // SI está mueble al alcance, lo ponemos como target
                            if (station != null)
                            {
                                activeAgent.targetStation = station;
                                // Marcamos parent 1 y parent 2 como terminados
                                currentNode.parent1.done = true;
                                currentNode.parent2.done = true;

                                // Si es nodo final, siguiente receta
                                if (currentNode.child == null)
                                {
                                    nextRecipie();
                                }
                            }
                            // SI no, lo ponemos al alcance del compañero
                            else
                            {
                                // Si no hay, tenemos un problema...
                                commonTable = getCommonStation(4);
                                activeAgent.targetStation = commonTable;
                                step = AIStep.STEP2;
                            }
                        }
                        //Situación producto-producto
                        else
                        {
                            // Lo dejamos en una mesa común
                            commonTable = getCommonStation(4);
                            activeAgent.targetStation = commonTable;
                            productJoin = true;
                            step = AIStep.STEP2;
                        }

                        activeAgent.startBehaviour();
                    }
                    break;

                case AIStep.STEP2:
                    {
                        // Si es situación producto-producto
                        if (productJoin)
                        {
                            activeAgent = null;
                            for (int i = 0; i < agents.Length; i++)
                            {
                                // Si el agente 'i' tene el producto al alcance, pasa a ser el activeAgent
                                ProductInstance product = agents[i].reachableTracker.getProductOnReach(currentNode.parent2.id);
                                if (product != null)
                                {
                                    activeAgent = agents[i];
                                    activeAgent.targetProduct = product;
                                    break;
                                }
                            }
                            if (activeAgent == null)
                                break;

                            // Lo llevará a la mesa donde está el parent 1
                            activeAgent.targetStation = commonTable;
                        }
                        // Si es situación producto-mueble
                        else
                        {
                            activeAgent = null;
                            for (int i = 0; i < agents.Length; i++)
                            {
                                // Si el agente 'i' tene el mueble al alcance, pasa a ser el activeAgent
                                // Llevará el commonProduct al mueble
                                StationInstance station = agents[i].reachableTracker.getStationOnReach(currentNode.parent2.id, false);
                                if (station != null)
                                {
                                    activeAgent = agents[i];
                                    activeAgent.targetStation = station;
                                    activeAgent.targetProduct = commonProduct;
                                }
                            }
                            if (activeAgent == null)
                                break;
                        }

                        // Marcamos parent 1 y parent 2 como terminados
                        currentNode.parent1.done = true;
                        currentNode.parent2.done = true;

                        activeAgent.startBehaviour();

                        step = AIStep.STEP1;
                    }
                    break;
                
                // EN step 3 entregamos los productos finales
                case AIStep.STEP3:
                    //Miramos qué agente tiene acceso al punto de entrega
                    activeAgent = null;
                    for (int i = 0; i < agents.Length; i++)
                    {
                        // Si el agente 'i' tene el producto al alcance, pasa a ser el activeAgent
                        DeliverySpot deliverySpot = agents[i].reachableTracker.getDeliverySpotOnReach();
                            if (deliverySpot != null)
                        {
                            activeAgent = agents[i];
                            activeAgent.targetDeliverySpot = deliverySpot;
                            break;
                        }
                    }
                    // SI no hay... tenemos un problema houston
                    if (activeAgent == null)
                        break;

                    activeAgent.targetProduct = commonProduct;
                    activeAgent.delivering = true;
                    activeAgent.startBehaviour();

                    step = AIStep.STEP1;

                    break;
            }

        }
    }

    AIAgent getReachableAgent(int productId)
    {

        foreach(AIAgent agent in agents)
        {
            // Si el agente 'i' tene el producto al alcance, pasa a ser el activeAgent
            ProductInstance product = agent.reachableTracker.getProductOnReach(currentNode.parent1.id);
            if (product != null)
            {
                return agent;
            }
        }
        return null;
    }

    public void nextRecipie()
    {
        // Se coje la siguiente receta que toque
        // De momento aleatorio
        int idx = Mathf.FloorToInt(Random.Range(0, 3.99f));
        currentRecipie = recipies[idx].copySelf(null);
    }

    public StationInstance getCommonStation(int id)
    {
        List<StationInstance> common = null;
        foreach (AIAgent agent in agents)
        {
            if (common == null)
                common = agent.reachableTracker.getStationListOnReach(id, false);
            else
                common = common.Intersect(agent.reachableTracker.getStationListOnReach(id, false)).ToList();
        }

        return common[0];
    }

    // Se generan los ároles de recetas que usan los IAagents
    private void genProductTrees()
    {
        foreach (Product product in ProductManager.finalProducts)
        {
            recipies.Add(createProdNode(product.id, null));
        }
    }

    // O(N^99999), pero nos vale de momento
    private RecipieNode createProdNode(int id, RecipieNode child)
    {
        RecipieNode node = new RecipieNode(id);
        node.isStation = false;
        node.child = child;

        // Miramos si viene de un product
        foreach (Product product in ProductManager.productBlueprints.Values)
        {
            foreach (Transition transition in product.transitions)
            {
                if (transition.dst == id)
                {
                    // Encontrado padre
                    if (node.parent1 == null)
                    {
                        node.parent1 = createProdNode(product.id, node);
                        node.parent2 = createProdNode(transition.src, node);
                    }
                }
            }
        }
        // Si no viene de un product, viene de un station
        if (node.parent1 == null)
        {
            foreach (Station station in ProductManager.stationBlueprints.Values)
            {
                foreach (Transition transition in station.transitions)
                {
                    // Encontrado padre
                    if (transition.dst == id)
                    {
                        node.parent1 = createProdNode(transition.src, node);
                        node.parent2 = createStatNode(station.id, node);
                    }
                }
            }
        }

        // Este nodo se colapsa para no confundir a la IA
        if (ProductManager.productBlueprints[id].AIcollapse)
        {
            node = node.parent1;
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
