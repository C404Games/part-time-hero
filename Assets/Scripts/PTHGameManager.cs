using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PTHGameManager : MonoBehaviourPunCallbacks
{
    public string avatarPrefabName = "Player";

    public List<GameObject> spawnPoints;

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("El modo offline esta: " + PhotonNetwork.OfflineMode);
        PhotonNetwork.OfflineMode = true;
        Debug.Log("Ahora está: " + PhotonNetwork.OfflineMode);
    }
    void Start()
    {
        if(PhotonNetwork.OfflineMode || PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.Instantiate(
            Path.Combine("Characters", avatarPrefabName),
            spawnPoints[0].transform.position,
            spawnPoints[0].transform.rotation
            );
            PhotonNetwork.Instantiate(
            Path.Combine("Characters", avatarPrefabName),
            spawnPoints[1].transform.position,
            spawnPoints[1].transform.rotation
            );
        }
        else
        {
            PhotonNetwork.Instantiate(
            Path.Combine("Characters", avatarPrefabName), 
            spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position,
            spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.rotation
            );
        }

        if (PhotonNetwork.OfflineMode || PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(
            Path.Combine("AI", "IAManager"),
            spawnPoints[4].transform.position,
            spawnPoints[4].transform.rotation
            );

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
