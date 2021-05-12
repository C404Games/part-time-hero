using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AIStep
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
    StationInstance lastStation;
    ProductInstance commonProduct;
    ProductInstance secondaryProduct;

    MatchManager matchManager;

    public AIStep step;

    public bool productJoin = false;


    // Start is called before the first frame update
    void Start()
    {
        recipies = new List<RecipieNode>();

        matchManager = FindObjectOfType<MatchManager>();
        if (!matchManager == null)
        {
            if (matchManager.team2Dishes == null)
            {
                matchManager.team1Dishes = new List<MatchManager.Tuple<bool, int>>();
                matchManager.team2Dishes = new List<MatchManager.Tuple<bool, int>>();
            }
        }
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
        if (matchManager.isPaused)
            return;
        if (currentRecipie != null)
        {
            // Si el activeAgent ha terminado
            if (activeAgent == null || !activeAgent.busy)
            {
                // COmprobamos si nos han gitaneado el producto primario
                if (currentNode != null && commonProduct != null)
                {
                    bool stolen = true;
                    foreach (AIAgent agent in agents)
                    {
                        if (agent.reachableTracker.isProductOnReach(commonProduct))
                        {
                            stolen = false;
                            break;
                        }
                    }
                    // En ese caso, reiniciamos el paso
                    if (stolen)
                    {
                        //commonProduct = null;
                        resetNodeRecursive(currentNode);
                        //currentNode.parent1.parent1.done = false;
                        //currentNode.parent1.parent2.done = false;
                    }
                }
                // COmprobamos si nos han gitaneado el producto secundario
                if (currentNode != null && secondaryProduct != null)
                {
                    bool stolen = true;
                    foreach (AIAgent agent in agents)
                    {
                        if (agent.reachableTracker.isProductOnReach(secondaryProduct))
                        {
                            stolen = false;
                            break;
                        }
                    }
                    // En ese caso, reiniciamos el paso
                    if (stolen)
                    {
                        //secondaryProduct = null;
                        resetNodeRecursive(currentNode);
                        //currentNode.parent2.parent1.done = false;
                        //currentNode.parent2.parent2.done = false;
                    }
                }

                switch (step)
                {
                    case AIStep.STEP1:
                        {

                            currentNode = currentRecipie.getLeaf();

                            // Hemos terminado a receta
                            if (currentNode == null)
                            {
                                step = AIStep.STEP3;
                                nextRecipie();
                                break;
                            }

                            // El parent 1 del nodo está garantizado que sea Producto
                            agents = agents.ToList().OrderBy(x => Random.value).ToArray();
                            activeAgent = null;
                            for (int i = 0; i < agents.Length; i++)
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

                            secondaryProduct = commonProduct;

                            commonProduct = activeAgent.targetProduct;

                            // Situación producto-mueble
                            if (currentNode.parent2.isStation)
                            {
                                StationInstance station = null;
                                if (currentNode.hasPre)
                                    station = lastStation;
                                else
                                {
                                    station = activeAgent.reachableTracker.getStationOnReach(currentNode.parent2.id, false);
                                    if (currentNode.parent1.isPre)
                                        lastStation = station;
                                }
                                // SI está mueble al alcance, lo ponemos como target
                                if (station != null)
                                {
                                    activeAgent.targetStation = station;
                                    // Marcamos parent 1 y parent 2 como terminados
                                    currentNode.parent1.done = true;
                                    currentNode.parent2.done = true;

                                    // Si es nodo final, siguiente receta
                                    /*
                                    if (currentNode.child == null)
                                    {
                                        step = AIStep.STEP3;
                                        nextRecipie();
                                    }
                                    */
                                    
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
                                        productJoin = false;
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
                        {
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

                            activeAgent.targetProduct = commonProduct != null? commonProduct : secondaryProduct;
                            activeAgent.delivering = true;
                            activeAgent.startBehaviour();

                            step = AIStep.STEP1;
                        }
                        break;
                }

            }
        }
        else
        {
            nextRecipie();
        }
    }

    public void resetStep()
    {
        step = AIStep.STEP1;
        resetNodeRecursive(currentNode);
    }

    public void resetNodeRecursive(RecipieNode node)
    {
        bool found = true;
        if (!node.isStation)
        {
            found = false;
            foreach (AIAgent agent in agents)
            {
                if (agent.reachableTracker.getProductOnReach(node.id) != null)
                {
                    found = true;
                    break;
                }
            }
        }
        if (node.parent1 != null)
        {
            node.parent1.done = found;
            resetNodeRecursive(node.parent1);
        }
        if (node.parent2 != null)
        {
            node.parent2.done = found;
            resetNodeRecursive(node.parent2);
        }
    }

    public void nextRecipie()
    {
        // Se coje la siguiente receta que toque
        //int idx = Mathf.Clamp(Random.Range(0, recipies.Count), 0, recipies.Count-1);
        //int idx = 3;
        if (matchManager.team2Dishes.Count > 0)
        {
            int id = matchManager.team2Dishes[0].Item2;
            currentRecipie = recipies.Find(n => n.id == id).copySelf(null);
        }
        else
            currentRecipie = null;
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
        if(common == null)
            return null;
        return common[0];
    }

    // Se generan los ároles de recetas que usan los IAagents
    private void genProductTrees()
    {
        foreach (int productId in ProductManager.finalProducts.Keys)
        {
            recipies.Add(createProdNode(productId, null));
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
                        
                        if(transition.pre >= 0)
                        {
                            node.hasPre = true;
                            node.parent2.parent1 = createProdNode(transition.pre, node.parent2);
                            node.parent2.parent2 = createStatNode(station.id, node.parent2);
                            node.parent2.parent1.isPre = true;
                        }
                        
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
