using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station
{
    public int id;
    public string name;
    public bool auto;
    public List<Transition> transitions;

    public Station(int id, string name, bool auto, List<Transition> transitions)
    {
        this.id = id;
        this.name = name;
        this.auto = auto;
        this.transitions = transitions;
    }
}
