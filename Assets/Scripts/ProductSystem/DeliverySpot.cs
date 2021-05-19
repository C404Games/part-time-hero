using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour
{

    MatchManager matchManager;
    MenuBehaviour menuBehaviour;

    ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        matchManager = FindObjectOfType<MatchManager>();
        particles = transform.GetComponentInChildren<ParticleSystem>();
    }

    public void deliverProduct(int team, ProductInstance product)
    {
        if (product.getProductType() == ProductType.FINAL && matchManager.deliverProduct(team, product.id))
        {
            particles.Stop();
            particles.Play();
        }
        PhotonNetwork.Destroy(product.gameObject);
    }
}
