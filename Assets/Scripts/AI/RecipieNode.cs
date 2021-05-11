﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipieNode
{


    public int id;

    public bool isStation;

    public bool done = false;

    public bool hasPre = false;
    public bool isPre = false;

    public RecipieNode parent1, parent2;

    public RecipieNode child;

    public RecipieNode(int id)
    {
        this.id = id;
    }


    public RecipieNode getLeaf()
    {
        int level1 = 0, level2 = 0;
        RecipieNode leaf1 = null, leaf2 = null;
        if (parent1 != null && !parent1.done)
            leaf1 = parent1.getLeaf(ref level1);
        if (parent2 != null && !parent2.done)
            leaf2 = parent2.getLeaf(ref level2);


        RecipieNode chosen;
        if (leaf1 == null || leaf2 == null)
            chosen = this;
        if (leaf1 == null)
            chosen = leaf2;
        if (leaf2 == null)
            chosen = leaf1;
        else
            chosen = level1 >= level2 ? leaf1 : leaf2;

        if(chosen == null)
            return null;

        return chosen.child;

    }

    public RecipieNode copySelf(RecipieNode child)
    {
        RecipieNode node = new RecipieNode(id);
        node.isStation = isStation;
        node.child = child;
        node.isPre = isPre;
        node.hasPre = hasPre;
        if(parent1 != null)
            node.parent1 = parent1.copySelf(node);
        if(parent2 != null)
            node.parent2 = parent2.copySelf(node);
        return node;
    }

    private RecipieNode getLeaf(ref int level)
    {
        level++;
        int level1 = level, level2 = level;
        RecipieNode leaf1 = null, leaf2 = null;

        if (parent1 != null && !parent1.done)
            leaf1 = parent1.getLeaf(ref level1);
        if (parent2 != null && !parent2.done)
            leaf2 = parent2.getLeaf(ref level2);

        if (leaf1 == null || leaf2 == null)
            return this;
        if (leaf1 == null)
        {
            level = level2;
            return leaf2;
        }
        if (leaf2 == null)
        {
            level = level1;
            return leaf1;
        }
        if(level1 >= level2)
        {
            level = level1;
            return leaf1;
        }
        else
        {
            level = level2;
            return leaf2;
        }
    }    

}
