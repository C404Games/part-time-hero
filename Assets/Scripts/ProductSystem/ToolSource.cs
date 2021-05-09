using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ToolSource : MonoBehaviour
{

    public int toolId;

    public GameObject prefab;

    public GameObject heldTool;

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.OfflineMode || PhotonNetwork.LocalPlayer.ActorNumber != 1)
        {
            return;
        }
        if (heldTool == null || heldTool.GetComponent<ProductInstance>().getHolder() != transform)
        {
            heldTool = PhotonNetwork.Instantiate(
                Path.Combine("Escenario", "Common", "Product"),
                transform.position, 
                Quaternion.identity
                );
            heldTool.GetComponent<Rigidbody>().isKinematic = true;
            ProductInstance product = heldTool.GetComponent<ProductInstance>();
            product.id = toolId;
            product.setHolder(transform);
        }

    }

    public void returnTool(ProductInstance tool)
    {
        if (heldTool != null)
            Destroy(heldTool);
        heldTool = tool.gameObject;
        heldTool.GetComponent<Rigidbody>().isKinematic = true;
        tool.setHolder(transform);
    }

}
