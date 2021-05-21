using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            GetComponent<Button>().interactable = false;
            gameObject.SetActive(false);
        }
    }
}
