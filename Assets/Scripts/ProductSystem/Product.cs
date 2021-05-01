using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ProductType
{
    RAW,
    MID,
    FINAL,
    TOOL
}

public class Product
{
    public int id;
    public string name;

    public ProductType type;

    public int difficulty;

    public GameObject appearence;

    public List<Transition> transitions;

    public bool AIcollapse;

    public Product(int id, string name, ProductType type, int difficulty, GameObject appearence, List<Transition> transitions, bool AIcollapse)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.difficulty = difficulty;
        this.appearence = appearence;
        this.transitions = transitions;
        this.AIcollapse = AIcollapse;
    }
}