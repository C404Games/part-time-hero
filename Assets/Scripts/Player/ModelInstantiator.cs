using System.IO;

using UnityEngine;
using Photon.Pun;

public class ModelInstantiator : MonoBehaviour
{
    public bool isMain;

    public Transform wrapper;

    string[] models = {
        "p1",
        "p2",
        "p3"
    };

    // Start is called before the first frame update
    void Awake()
    {
        int idx;
        if (isMain)
        {
            idx = FindObjectOfType<universalParameters>().getModel();
        }
        else
        {
            idx = Random.Range(0, models.Length - 1);
        }

        GameObject model = PhotonNetwork.Instantiate(Path.Combine("Characters", "AnimatedModels", models[idx]), Vector3.zero, Quaternion.identity);
        model.transform.parent = wrapper;
        model.transform.localPosition = new Vector3(0, 0, 0);
        model.transform.localRotation = Quaternion.identity;
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        model.GetComponent<SwordVisibility>().playerMovement = playerMovement;
        playerMovement.animator = model.GetComponent<Animator>();
    }
}
