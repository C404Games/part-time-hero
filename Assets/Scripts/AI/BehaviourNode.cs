using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    AND,
    OR,
    LEAF
}

public class BehaviourNode
{
    List<BehaviourNode> children;

    public delegate bool performAction(AIAgent agent);

    public performAction action;

    NodeType type;

    public BehaviourNode(NodeType type, performAction action, List<BehaviourNode> children)
    {
        this.type = type;
        this.action = action;
        this.children = children;
    }

    public bool doTree(AIAgent agent)
    {
        foreach(BehaviourNode node in children)
        {
            bool success = node.doTree(agent);
            if (type == NodeType.AND && !success)
                return false;
            else if (type == NodeType.OR && success)
                return true;
        }
        return action(agent);
    }

}
