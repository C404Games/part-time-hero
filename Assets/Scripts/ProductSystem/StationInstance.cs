using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInstance : MonoBehaviour
{

    public int id;

    public Transform waitPosition;

    public GameObject mainModel;
    public GameObject brokenModel;

    public ProductInstance heldProduct;

    Station blueprint;

    int health;

    RadialClockController clockController;

    bool busy = false;
    public float transitionTime;
    public float currentTransitionTime;
    int transitionDst;

    bool changeSpeed = false;
    float speedFactor = 1;
    float currentChangeSpeedTime;
    float maxChangeSpeedTime;

    float repairTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        clockController = FindObjectOfType<RadialClockController>();
        blueprint = ProductManager.stationBlueprints[id];
        health = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (changeSpeed)
        {
            if(currentChangeSpeedTime < maxChangeSpeedTime)
            {
                currentChangeSpeedTime += Time.deltaTime;
            }
            else
            {
                changeSpeed = false;
                speedFactor = 1;
            }
        }
        if (busy)
        {
            if (currentTransitionTime < transitionTime)
            {
                currentTransitionTime += Time.deltaTime * speedFactor;
            }
            else
            {
                busy = false;
                // Si tiene vida, termina cocinado
                if (health > 0)
                {
                    heldProduct.transformProduct(transitionDst);
                    activate();
                }
                // Si no, se repara
                else
                {
                    repair();
                }                
            }
        }
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
    public float putProduct(ProductInstance product)
    {
        if (health > 0 && !busy)
        {
            if (heldProduct != null)
            {
                if (heldProduct.applyResource(product.id))
                {
                    Destroy(product.gameObject);
                    return activate();
                }
                return -1;
            }
            else
            {
                heldProduct = product;
                heldProduct.setHolder(transform);
                return activate();
            }
        }
        else
        {
            return -1;
        }

    }

    // Devuelve el tiempo en segundos si NO es auto. Devuelve 0 si es auto.
    public float activate()
    {
        if(health > 0 && !busy && heldProduct != null)// && isReachable(origin))
        {
            foreach(Transition transition in blueprint.transitions)
            {
                if(heldProduct.id == transition.src)
                {
                    busy = true;

                    float time = transition.time;
                    
                    busy = true;
                    transitionTime = time;
                    currentTransitionTime = 0;
                    transitionDst = transition.dst;

                    return blueprint.auto ? 0 : time;
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

    public float interact(CatcherScript catcher)
    {
        if (health > 0)
        {
            if (catcher.getHeldProduct() == null)
            {
                catcher.holdProduct(heldProduct);
                heldProduct = null;
            }
            else if (heldProduct == null)
            {
                return putProduct(catcher.getHeldProduct());
            }
            return 0;
        }
        else
        {
            transitionTime = repairTime;
            currentTransitionTime = 0;
            busy = true;
            return repairTime;
        }
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
        if (health > 0)
        {
            health -= h;
            if (health <= 0)
            {
                mainModel.SetActive(false);
                brokenModel.SetActive(true);
                if(heldProduct != null)
                    Destroy(heldProduct.gameObject);
                busy = false;
            }
        }
    }

    public void repair()
    {
        health = 5;
        mainModel.SetActive(true);
        brokenModel.SetActive(false);
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

    public void startSpeedChange(float factor, float time)
    {
        changeSpeed = true;
        speedFactor = factor;
        maxChangeSpeedTime = time;
        currentChangeSpeedTime = 0;
    }    

}
