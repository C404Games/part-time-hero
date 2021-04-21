using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSource : MonoBehaviour
{

    public int toolId;

    public GameObject prefab;

    public GameObject heldTool;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (heldTool == null || heldTool.GetComponent<ProductInstance>().getHolder() != transform)
        {
            heldTool = Instantiate(prefab, transform.position, Quaternion.identity);
            heldTool.GetComponent<Rigidbody>().isKinematic = true;
            ProductInstance product = heldTool.GetComponent<ProductInstance>();
            product.id = toolId;
            product.setHolder(transform);

        }
    }

}
