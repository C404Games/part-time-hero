using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductInstance : MonoBehaviour
{
    public int id;

    public bool held;

    private Product blueprint;

    private GameObject appearence;

    private float clampSpeed = 10;

    #region MonoBehavior
    // Start is called before the first frame update
    void Start()
    {
        held = false;
        blueprint = ProductManager.productBlueprints[id];
        updateAppearence();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent != null)
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * clampSpeed);
    }
    #endregion

    public ProductType getProductType()
    {
        return blueprint.type;
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
