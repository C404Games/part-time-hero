﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(ClickMovement))]

public class ToggleControl : MonoBehaviour
{
    public bool on;

    PlayerMovement playerMovement;
    ClickMovement clickMovement;
    CatcherScript catcher;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.OfflineMode)
        {
            clickMovement = GetComponent<ClickMovement>();
            playerMovement = GetComponent<PlayerMovement>();
            catcher = transform.GetComponentInChildren<CatcherScript>();
            playerMovement.active = on;
            clickMovement.active = on;
            catcher.active = on;
        }
    }

    public void toggle()
    {
        if (PhotonNetwork.OfflineMode)
        {
            on = !on;
            playerMovement.active = on;
            clickMovement.active = on;
            catcher.active = on;
        }
    }

}
