using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInstance : MonoBehaviour
{

    public int id;

    public Transform waitPosition;

    public ProductInstance heldProduct;

    Station blueprint;

    int health;

    RadialClockController clockController;

    bool busy = false;

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

    public Vector3 getWaitPos(Vector3 currentPos)
    {
        if (isInFront(currentPos))
        {
            return waitPosition.position;
        }
        else
        {
            Vector3 dir = transform.position - waitPosition.position;
            return transform.position + dir;
        }
    }

    public Vector3 getWaitRot(Vector3 currentPos)
    {
        if (isInFront(currentPos))
        {
            return transform.position - waitPosition.position;
        }
        else
        {
            Vector3 dir = transform.position - waitPosition.position;
            Vector3 pos = transform.position + dir;
            return transform.position - pos;
        }
    }

    // Devuelve el tiempo en segundos si NO es auto. Devuelve 0 si es auto. Devuelve -1 si no se puede dejar
    public float putProduct(ProductInstance product, Vector3 origin)
    {
        if (!busy)
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
                //heldProduct.transform.parent = transform;
                heldProduct.setHolder(transform);
                return activate(origin);
            }
        }
        else
        {
            return -1;
        }

    }

    // Devuelve el tiempo en segundos si NO es auto. Devuelve 0 si es auto.
    public float activate(Vector3 origin)
    {
        if(!busy && heldProduct != null)// && isReachable(origin))
        {
            foreach(Transition transition in blueprint.transitions)
            {
                if(heldProduct.id == transition.src)
                {
                    busy = true;
                    StartCoroutine(heldProduct.transformProduct(transition.dst, transition.time));

                    if(transition.time > 0)
                        clockController.startClock(this, transition.time);
                    
                    StartCoroutine(reactivate(transition.time + 0.1f));
                    StartCoroutine(freeStation(transition.time - 0.1f));

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

    // Devuelve true si pos está en frente
    public bool isInFront(Vector3 position)
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

    public bool isOccupied()
    {
        return heldProduct != null;
    }

    public bool isBusy()
    {
        return busy;
    }

    private IEnumerator reactivate(float time)
    {
        yield return new WaitForSeconds(time);
        activate(waitPosition.position);
    }

    private IEnumerator freeStation(float time)
    {
        yield return new WaitForSeconds(time);
        busy = false;
    }

}
