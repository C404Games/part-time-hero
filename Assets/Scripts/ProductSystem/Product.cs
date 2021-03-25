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

    public GameObject appearence;

    public List<Transition> transitions;

    public int probabilidadAcumulada;

    public Product(int id, string name, ProductType type, GameObject appearence, List<Transition> transitions)
    {
        this.name = name;
        this.type = type;
        this.appearence = appearence;
        this.transitions = transitions;
    }
}