using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    public int src;
    public int dst;
    public float time;

    public Transition(int src, int dst, float time)
    {
        this.src = src;
        this.dst = dst;
        this.time = time;
    }
}
