﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

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
                 // Ir a mueble
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
                     return a.goToTargetToolSource();
                 }, null)
            }),

             // Ir a cinta
            new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                return a.goToBelt();
            }, null)
        })
    });

    BehaviourNode carryTree = new BehaviourNode(NodeType.OR, (AIAgent a) => { return true; }, new List<BehaviourNode>()
    {
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
                        return a.carryToTable();
                    }, null)
                }),

                // AND (->) 
                // Va con un station!
                // Llevar con ese station si se puede
                new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
                    return a.carryToPartnerStation();
                }, null)
                
            })
        }),
        // Es parent 2 (Product)
        // Llevar con su compañero
        new BehaviourNode(NodeType.LEAF, (AIAgent a)=>{
            return a.carryToTracked();
        }, null)
    });

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
