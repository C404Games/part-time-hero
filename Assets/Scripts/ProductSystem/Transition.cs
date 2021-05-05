using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    public int src;
    public int dst;
    public float time;
    public bool auto;

    public Transition(int src, int dst, float time, bool auto)
    {
        this.src = src;
        this.dst = dst;
        this.time = time;
        this.auto = auto;
    }
}
