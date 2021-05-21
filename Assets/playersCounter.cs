using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playersCounter : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if(null != PhotonNetwork.CurrentRoom)
        {
            GetComponent<Text>().text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        }
    }
}
