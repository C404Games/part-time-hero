using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class PhotonController : MonoBehaviourPunCallbacks
{
    public string playerName = "Chibitor";

    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Lista sala de espera")]
    public GameObject InsideRoomPanel;
    public GameObject PlayerListEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    //Definicion de nombre de usuario
    public void OnLoginButtonClicked()
    {

        if (!playerName.Equals(""))
        {
            Debug.Log("Uniendose a la red con el nombre " + playerName);
            PhotonNetwork.LocalPlayer.NickName = playerName;
            if(PhotonNetwork.ConnectUsingSettings())
                Debug.Log("Usuario " + playerName + " unido correctamente a la red");
            else
                Debug.Log("Error al conectar al usuario " + playerName + " a la red");
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    //Creación de sala
    public void OnCreateRoomButtonClicked()
    {
        string roomName = Random.Range(1000, 10000).ToString();

        byte maxPlayers = 4;
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
        options.IsVisible = false;
        Debug.Log("Creando sala: " + roomName);
        if(PhotonNetwork.CreateRoom(roomName, options, null))
            Debug.Log("Sala " + roomName + " creada con exito");
        else
            Debug.Log("Sala no crada");

    }

    //Unión a sala
    private LoadBalancingClient loadBalancingClient;

    /**
     * Union a sala aleatoria
     **/
    public void OnJoinRandomRoomButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /**
     * Union a sala especifica
     * */
    public void JoinInPrivateRoom(string nameEveryFriendKnows)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomName = nameEveryFriendKnows;
        enterRoomParams.RoomOptions = roomOptions;
        loadBalancingClient.OpJoinRoom(enterRoomParams);
    }

    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Room creation failed with error code {0} and error message {1}", returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.parent = InsideRoomPanel.transform;
            
            entry.transform.localPosition = new Vector3(entry.transform.position.x, entry.transform.position.y, 0);
            entry.transform.localScale = Vector3.one;

            entry.transform.localRotation = Quaternion.Euler(Vector3.zero);

            entry.GetComponent<PlayerInList>().Initialize(p.NickName, p.ActorNumber);

            playerListEntries.Add(p.ActorNumber, entry);
        }
    }

    //Salida de sala
    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    //Busqueda de salas


    //Administración de sala
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }


    //Lista de salas publicas
    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            //entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, entry);
        }
    }

    //Control de sala de espera

    public override void OnLeftRoom()
    {
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.parent = InsideRoomPanel.transform;

        entry.transform.localPosition = new Vector3(entry.transform.position.x, entry.transform.position.y, 0);
        entry.transform.localScale = Vector3.one;
        entry.transform.localRotation = Quaternion.Euler(Vector3.zero);

        entry.GetComponent<PlayerInList>().Initialize(newPlayer.NickName, newPlayer.ActorNumber);

        playerListEntries.Add(newPlayer.ActorNumber, entry);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
    }
}
