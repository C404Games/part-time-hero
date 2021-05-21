using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PTHGameManager : MonoBehaviourPunCallbacks
{
    public string avatarPrefabName = "Player";

    public List<GameObject> charactersList;
    //public List<GameObject> animatorsList;
    public List<GameObject> catcherList;
    public GameObject botList;


    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("El modo offline esta: " + PhotonNetwork.OfflineMode);

        //if (!PhotonNetwork.OfflineMode && PhotonNetwork.CurrentRoom.Players[0].ActorNumber != 1)
        //{
        //    return;
        //}
        charactersList[2].SetActive(false);
        charactersList[3].SetActive(false);
        if (PhotonNetwork.OfflineMode || PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {   
            charactersList[1].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(2));
            catcherList[1].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(2));

        }
        if (PhotonNetwork.CurrentRoom.PlayerCount > 2)
        {
            botList.SetActive(false);

            charactersList[2].SetActive(true);
            charactersList[3].SetActive(true);

            charactersList[2].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(3));
            charactersList[3].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(4));

            //animatorsList[2].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(3));
            //animatorsList[3].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(4));

            catcherList[2].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(2));
            catcherList[3].GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(2));
        }

        if (PhotonNetwork.OfflineMode || PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            //PhotonNetwork.Instantiate(
            //Path.Combine("AI", "IAManager"),
            //spawnPoints[4].transform.position,
            //spawnPoints[4].transform.rotation
            //);

        }
    }
    void Start()
    {
        

    }
}
