using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hideButton : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (null != PhotonNetwork.CurrentRoom && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            GetComponent<Image>().color = new Color(
                GetComponent<Image>().color.r,
                GetComponent<Image>().color.b,
                GetComponent<Image>().color.g,
                1f
                );
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Image>().color = new Color(
                   GetComponent<Image>().color.r,
                   GetComponent<Image>().color.b,
                   GetComponent<Image>().color.g,
                   0.5f
                   );
            GetComponent<Button>().interactable = false;
        }
    }
}
