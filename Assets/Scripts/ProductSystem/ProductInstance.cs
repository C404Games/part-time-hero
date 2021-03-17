using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductInstance : MonoBehaviour
{
    public int id;

    private Product blueprint;

    #region MonoBehavior
    // Start is called before the first frame update
    void Start()
    {
        blueprint = ProductManager.productBlueprints[id];
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion


    // Si hay alguna transición con este producto, se hace
    public bool applyResource(Product resource)
    {
        foreach (Transition t in blueprint.transitions)
        {
            if (t.src == resource.id)
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
        Destroy(blueprint.appearence);
        blueprint = ProductManager.productBlueprints[id];
    }
}
