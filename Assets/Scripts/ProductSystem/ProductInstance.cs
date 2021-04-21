using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductInstance : MonoBehaviour
{
    public int id;

    private Transform holder;

    private Product blueprint;

    private GameObject appearence;

    private float clampSpeed = 10;

    private Rigidbody rigidbody;

    private float clampTime = 0.5f;
    private float currentClampTime = 0;

    #region MonoBehavior
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        blueprint = ProductManager.productBlueprints[id];
        updateAppearence();
    }

    // Update is called once per frame
    void Update()
    {
        //if(transform.parent != null)

        if(holder != null)
        {
            Vector3 newPos = new Vector3();
            if (currentClampTime < clampTime) {
                currentClampTime += Time.deltaTime;
                newPos = Vector3.Lerp(transform.position, holder.transform.position, clampSpeed * Time.deltaTime);
            }
            else
            {
                newPos = holder.transform.position;
            }
            rigidbody.MovePosition(newPos);
        }

    }
    #endregion

    public ProductType getProductType()
    {
        return blueprint.type;
    }

    public void setHolder(Transform holder )
    {
        currentClampTime = 0;
        this.holder = holder;
    }

    public Transform getHolder()
    {
        return holder;
    }

    public bool isHeld()
    {
        return holder != null && holder.GetComponent<StationInstance>() != null;
    }


    // Si hay alguna transición con este producto, se hace
    public bool applyResource(int resourceId)
    {
        foreach (Transition t in blueprint.transitions)
        {
            if (t.src == resourceId)
            {
                StartCoroutine(transformProduct(t.dst, t.time));
                return true;
            }
        }
        return false;
    }

    public IEnumerator transformProduct(int dst, float time)
    {
        yield return new WaitForSeconds(time);
        id = dst;
        blueprint = ProductManager.productBlueprints[id];
        updateAppearence();
    }

    private void updateAppearence()
    {
        if(appearence != null)
            Destroy(appearence);
        appearence = Instantiate(blueprint.appearence);
        appearence.transform.parent = transform;

        appearence.transform.localPosition = new Vector3(0, 0, 0);
    }
}
