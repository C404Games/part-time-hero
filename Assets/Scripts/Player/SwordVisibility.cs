using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordVisibility : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject sword;

    // Start is called before the first frame update
    void Start()
    {
        sword.SetActive(false);
    }

    public void animationStart()
    {
        sword.SetActive(true);
    }

    public void animationEnd() {
        sword.SetActive(false);
        playerMovement.attackBusy = false;
    }

}
