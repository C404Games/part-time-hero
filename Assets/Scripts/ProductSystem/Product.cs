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

    public string appearence;

    public List<Transition> transitions;

    public bool AIcollapse;

    public float time;

    public Product(int id, string name, ProductType type, int difficulty, string appearence, List<Transition> transitions, bool AIcollapse, float time)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.difficulty = difficulty;
        this.appearence = appearence;
        this.transitions = transitions;
        this.AIcollapse = AIcollapse;
        this.time = time;
    }
}