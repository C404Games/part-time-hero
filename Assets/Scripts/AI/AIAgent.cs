using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    WAITING,
    GRABBING_RESOURCE,
    CARRYING_RESOURCE,
    ELABORATING,
    DELIVERING,
    FIGHTING,
    REPAIRING
}

public class AIAgent : MonoBehaviour
{

    bool busy = false;

    State state = State.WAITING;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.WAITING:
                break;
            case State.GRABBING_RESOURCE:
                break;
            case State.ELABORATING:
                break;
            case State.DELIVERING:
                break;
        }
    }

    public bool isBusy()
    {
        return busy;
    }
}
