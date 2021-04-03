using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInstance : MonoBehaviour
{

    public int id;

    public Transform waitPosition;

    ProductInstance heldProduct;

    Station blueprint;

    int health;

    RadialClockController clockController;

    // Start is called before the first frame update
    void Start()
    {
        clockController = FindObjectOfType<RadialClockController>();
        blueprint = ProductManager.stationBlueprints[id];
        health = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getWaitPos()
    {
        return waitPosition.position;
    }

    public Vector3 getWaitRot()
    {
        return transform.position - waitPosition.position;
    }

    // Devuelve el tiempo en segundos si NO es auto. Devuelve 0 si es auto. Devuelve -1 si no se puede dejar
    public float putProduct(ProductInstance product, Vector3 origin)
    {

        if (heldProduct != null) 
        {
            if (heldProduct.applyResource(product.id))
            {
                Destroy(product.gameObject);
                return 0;
            }
            return -1;
        }
        else
        {
            heldProduct = product;
            heldProduct.transform.parent = transform;
            return activate(origin);
        }

    }

    // Devuelve el tiempo en segundos si NO es auto. Devuelve 0 si es auto.
    public float activate(Vector3 origin)
    {
        if(heldProduct != null && isReachable(origin))
        {
            foreach(Transition transition in blueprint.transitions)
            {
                if(heldProduct.id == transition.src)
                {
                    StartCoroutine(heldProduct.transformProduct(transition.dst, transition.time));

                    if(transition.time > 0)
                        clockController.startClock(this, transition.time);
                    if(transition.time == 0)
                        StartCoroutine(reactivate(transition.time + 0.1f));

                    return blueprint.auto ? 0 : transition.time;
                }
            }
        }
        return 0;
    }

    public ProductInstance takeProduct()
    {
        ProductInstance product = heldProduct;
        heldProduct = null;
        return product;
    }

    // Devuelve true si se puede utilizar desde la posición que se le pasa
    public bool isReachable(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        float cosAngle = Vector3.Dot(dir, waitPosition.localPosition);
        return cosAngle > 0;
    }

    public void takeHealth(int h)
    {
        health -= h;
        if(health <= 0)
        {
            // Cambiar apariencia
        }
    }

    public int getHealth()
    {
        return health;
    }

    private IEnumerator reactivate(float time)
    {
        yield return new WaitForSeconds(time);
        activate(waitPosition.position);
    }

}
