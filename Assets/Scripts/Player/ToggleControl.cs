using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(ClickMovement))]

public class ToggleControl : MonoBehaviour
{
    public bool on;

    public GameObject arrow;

    PlayerMovement playerMovement;
    ClickMovement clickMovement;
    CatcherScript catcher;

    ToggleControl partner;

    // Start is called before the first frame update
    void Start()
    {
        clickMovement = GetComponent<ClickMovement>();
        playerMovement = GetComponent<PlayerMovement>();
        catcher = transform.GetComponentInChildren<CatcherScript>();
        playerMovement.active = on;
        clickMovement.active = on;
        catcher.active = on;
        arrow.SetActive(on);
        partner = FindObjectOfType<ToggleControl>();
    }

    private void Update()
    {
        arrow.transform.LookAt(Camera.main.transform, Camera.main.transform.up);
    }

    public void toggle()
    {
        on = !on;
        playerMovement.active = on;
        clickMovement.active = on;
        catcher.active = on;
        arrow.SetActive(on);
    }

    public void togglePartner()
    {
        on = !on;
        playerMovement.active = on;
        clickMovement.active = on;
        catcher.active = on;
        arrow.SetActive(on);
        partner.toggle();
    }

}
