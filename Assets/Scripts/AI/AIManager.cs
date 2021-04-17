using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    // COmportamiento al buscar objeto
    BehaviourNode fetchTree = new BehaviourNode(NodeType.AND, (AIAgent a) => { return true; }, new List<BehaviourNode>() {
        
        // Saco nodo
        new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
            return a.updateTarget();
        }, null), 

        // OR (?)
        new BehaviourNode(NodeType.OR, (AIAgent a)=>{return true; }, new List<BehaviourNode>(){
             
            // AND (->)
            new BehaviourNode(NodeType.AND, (AIAgent a)=>{ return true; }, new List<BehaviourNode>(){

                // Está en mueble?
                 new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                     return a.isTargetHeld();
                 }, null),
                 // Ir a mueble si no está busy
                 new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                     return a.goToTargetStation();
                 }, null)
            }),

            // AND (->)
            new BehaviourNode(NodeType.AND, (AIAgent a)=>{ return true; }, new List<BehaviourNode>(){

                // Es herramienta?
                 new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                     return a.isTargetTool();
                 }, null),
                 // Ir a toolSource
                 new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                     return a.goToToolSource();
                 }, null)
            }),

             // Ir a cinta
            new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                return a.goToBelt();
            }, null)
        })
    });

    // Comportamiento al llevar objeto a donde toque
    BehaviourNode carryTree = new BehaviourNode(NodeType.OR, (AIAgent a) => { return true; }, new List<BehaviourNode>()
    {
        // AND (->)
        new BehaviourNode(NodeType.AND, (AIAgent a)=>{ return true; }, new List<BehaviourNode>(){

            // Es final?
            new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                return a.isFinalProduct();
            }, null),

            // OR (?)
            new BehaviourNode(NodeType.OR, (AIAgent a)=>{return true;},  new List<BehaviourNode>(){

                // SI tengo punto de entrego, lo llevo
                new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                    return a.deliverProduct();
                }, null),

                // SI no, lo llevo a una mesa común con el compi
                new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                    return a.carryToCommonTable();
                }, null),
            }),

            // Cojo nueva receta
            new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                return a.nextRecipie();
            }, null),
        }),

        // AND (->)
        new BehaviourNode(NodeType.AND, (AIAgent a)=>{ return true; }, new List<BehaviourNode>(){

            // Es parent 1?
            new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                return a.isParent1();
            }, null),

            // OR
            new BehaviourNode(NodeType.OR, (AIAgent a)=>{return true; }, new List<BehaviourNode>(){

                // AND (->)
                new BehaviourNode(NodeType.AND, (AIAgent a)=>{ return true; }, new List<BehaviourNode>(){

                    // Va con otro product?
                    new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                        return a.isJoinedWithProduct();
                    }, null),

                    // Llevar a mesa
                    new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                        return a.carryToCommonTable();
                    }, null)
                }),

                // Va con un mueble!
                // Llevar con ese mueble si se puede
                new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                    return a.carryToPartnerStation();
                }, null)                
            })
        }),
        // Es parent 2 (Product)
        // Llevar con su pareja
        new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
            return a.carryToTracked();
        }, null)
    });

    List<RecipieNode> recipies;

    // Mesas comunes siendo utilizadas (Para no mandar a dos agents a la misma)
    List<StationInstance> commonReservedTables;

    // Todos los AIAgent hacen la misma receta
    public RecipieNode currentRecipie;

    public AIAgent[] agents;

    public ReachableTracker[] areas;


    // Start is called before the first frame update
    void Start()
    {
        recipies = new List<RecipieNode>();
        genProductTrees();
        // Cojemos una receta cualquiera
        nextRecipie();

        int id = 0;
        foreach(AIAgent agent in agents)
        {
            agent.id = id++;
        }

        commonReservedTables = new List<StationInstance>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(AIAgent agent in agents)
        {
            if (!agent.isBusy())
            {
                switch (agent.state)
                {                    
                    case AIState.FETCH:
                        fetchTree.doTree(agent);
                        break;
                    case AIState.CARRY:
                        carryTree.doTree(agent);
                        break;
                }
            }
        }

        // Hay que acualizar las mesas comunes que ya no están reservadas
        commonReservedTables.RemoveAll(item => item.isOccupied());
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
        foreach (ReachableTracker area in areas)
        {
            if (common == null)
                common = area.getStationListOnReach(id, false);
            else
                common = common.Intersect(area.getStationListOnReach(id, false)).ToList();
        }

        foreach (StationInstance station in common)
        {
            if (!commonReservedTables.Contains(station))
            {
                commonReservedTables.Add(station);
                return station;
            }
        }

        return null;
    }

    // Se generan los ároles de recetas que usan los IAagents
    private void genProductTrees()
    {
        foreach(Product product in ProductManager.finalProducts)
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
                        node.parent2 = createProdNode(transition.src, node);
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
