using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour
{

    MatchManager matchManager;
    MenuBehaviour menuBehaviour;

    public GameObject dish1;
    public GameObject dish2;
    public GameObject dish3;
    public GameObject dish4;

    ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        matchManager = FindObjectOfType<MatchManager>();
        particles = transform.GetComponentInChildren<ParticleSystem>();
    }

    public void deliverProduct(int team, ProductInstance product)
    {
        int deliveryResult = matchManager.deliverProduct(team, product.id);
        if (product.getProductType() == ProductType.FINAL && deliveryResult >= 0)
        {
            // Avisar al manager que lleve eso
            PhotonNetwork.Destroy(product.gameObject);
            string currentPanelDishName = "Dish(Clone)";
            Transform teamDishPanel = dish1.transform;
            switch (deliveryResult)
            {
                case 0:
                    {
                        teamDishPanel = dish1.transform;
                        break;
                    }
                case 1:
                    {
                        teamDishPanel = dish2.transform;
                        break;
                    }
                case 2:
                    {
                        teamDishPanel = dish3.transform;
                        break;
                    }
                case 3:
                    {
                        teamDishPanel = dish4.transform;
                        break;
                    }
            }
            particles.Stop();
            particles.Play();
            Transform currentDishPanel = teamDishPanel.Find(currentPanelDishName);
            PhotonNetwork.Destroy(currentDishPanel.gameObject);
        }
        Destroy(product.gameObject);
    }
}
