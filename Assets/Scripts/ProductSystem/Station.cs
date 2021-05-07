using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station
{
    public int id;
    public string name;
    public List<Transition> transitions;
    public List<int> noHold;
    public bool breakable;

    public Station(int id, string name, List<Transition> transitions, List<int> noHold, bool breakable)
    {
        this.id = id;
        this.name = name;
        this.transitions = transitions;
        this.noHold = noHold;
        this.breakable = breakable;
    }
}
