using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public Transform optionsPosition;
    public float offsetY;
    public float movementVelocity = 1;

    private bool inScreen = false;

    // Update is called once per frame
    void Update()
    {
        if (inScreen)
            transform.position = Vector3.MoveTowards(
                transform.position, 
                new Vector3(transform.position.x, optionsPosition.position.y, transform.position.z), 
                Time.deltaTime * movementVelocity);
        else
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(transform.position.x, optionsPosition.position.y + offsetY, transform.position.z),
                Time.deltaTime * movementVelocity);
    }

    public void setInScreen(bool visible)
    {
        inScreen = visible;
    }

    public bool getInScreen()
    {
        return inScreen;
    }
}
