using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    public AIAgent[] agents;

    BehaviourNode rootNode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(AIAgent agent in agents)
        {
            if(!agent.isBusy())
                rootNode.doTree(agent);
        }
    }
}
