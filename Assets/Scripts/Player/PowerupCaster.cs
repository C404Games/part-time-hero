using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PowerupCaster : MonoBehaviour
{

    private AudioSource audioSource;

    private MatchManager matchManager;

    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        matchManager = FindObjectOfType<MatchManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.gameObject.tag == "PowerUp")
        {
            audioSource.Play();
            PowerUpBehaviour powerup = other.GetComponent<PowerUpBehaviour>();
            if (powerup != null)
            {
                matchManager.castPowerup(playerMovement, powerup.type);
            }
            PhotonNetwork.Destroy(other.transform.parent.gameObject);
        }
    }
}
