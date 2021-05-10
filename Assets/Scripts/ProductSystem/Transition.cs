using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    public int pre;
    public int src;
    public int dst;
    public float time;
    public bool auto;

    public Transition(int pre, int src, int dst, float time, bool auto)
    {
        this.pre = pre;
        this.src = src;
        this.dst = dst;
        this.time = time;
        this.auto = auto;
    }
}
