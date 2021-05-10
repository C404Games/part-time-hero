using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class PhotonController : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{

    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Lista sala de espera")]
    public GameObject InsideRoomPanel;
    public GameObject PlayerListEntryPrefab;

    [Header("Textos visibles")]
    public Text playerName;
    public GameObject idSala;
    public GameObject infoIdSala;

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public enum Map
    {
        Taberna = 1,
        Herrería = 2
    }
    [Header("Seleccion de mapa multijugador")]
    public Map opcion = Map.Taberna;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    //Definicion de nombre de usuario
    public void forcedLogin()
    {
        if (OnLoginButtonClicked())
            Debug.Log("Login forcoso correcto");
        else
            Debug.LogError("Error durante el login forzoso");
    }
    public bool OnLoginButtonClicked()
    {

        if (!"".Equals(PlayerPrefs.GetString("characterPlayerName", "")))
        {
            Debug.Log("Uniendose a la red con el nombre " + PlayerPrefs.GetString("characterPlayerName", ""));
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("characterPlayerName", "").ToUpper();
            if (PhotonNetwork.ConnectUsingSettings())
            {
                Debug.Log("Usuario " + PlayerPrefs.GetString("characterPlayerName", "") + " unido correctamente a la red");
                return true;
            }
            else
                Debug.Log("Error al conectar al usuario " + PlayerPrefs.GetString("characterPlayerName", "") + " a la red");
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
        return false;
    }

    public void onDisconnectButtonclicked()
    {
        Debug.Log("Jugador " + PlayerPrefs.GetString("characterPlayerName", "") + " desconectandose");
        PhotonNetwork.Disconnect();
        Debug.Log("Desconectado");

    }

    public bool reconnectPlayer()
    {
        Debug.Log("Jugador " + PlayerPrefs.GetString("characterPlayerName", "") + " reconectandose a la sala");
        return PhotonNetwork.Reconnect();
    }

    //Creación de sala
    public void OnCreateRoomButtonClicked()
    {
        string roomName = Random.Range(1000, 10000).ToString();

        byte maxPlayers = 4;
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
        options.IsVisible = true;
        options.IsOpen = true;
        Debug.Log("Creando sala: " + roomName);
        if (PhotonNetwork.CreateRoom(roomName, options, null))
        {
            Debug.Log("Sala " + roomName + " creada con exito");
            infoIdSala.GetComponent<Text>().text = roomName;
        }
        else
        {
            Debug.Log("Sala no crada");
            infoIdSala.GetComponent<Text>().text = "Error 404";
        }

    }

    //Unión a sala

    /**
     * Union a sala aleatoria
     **/
    public void OnJoinRandomRoomButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnPhotonRandomJoinFailed()
    {
        OnCreateRoomButtonClicked();
    }

    /**
     * Union a sala especifica
     * */
    public void JoinInPrivateRoom()
    {
        //Unirse sala
        Debug.Log("Entrando a la sala con id " + idSala.GetComponent<Text>().text);
        PhotonNetwork.JoinRoom(idSala.GetComponent<Text>().text);

        //Unirse a sala o crearla si no existe
        //RoomOptions roomOptions = new RoomOptions();
        //roomOptions.IsVisible = false;
        //PhotonNetwork.JoinOrCreateRoom(idSala.GetComponent<Text>().text, roomOptions, null);
    }

    /**
     * Union a lobby
     */
    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    /**
     * Metodo que se ejecuta cuado no se puede ingresar a una mesa
     */
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Error al entrar en la sala. {0} - {1}", returnCode, message);
    }

    /**
     * Metodo que se ejecuta cuado se consigue ingresar a una mesa
     */
    public override void OnJoinedRoom()
    {
        Debug.Log("Exito al entrar en la sala");
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        //Guardamos nombre de sala
        infoIdSala.GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.parent = InsideRoomPanel.transform;
            
            entry.transform.localPosition = new Vector3(entry.transform.position.x, entry.transform.position.y, 0);
            entry.transform.localScale = Vector3.one;

            entry.transform.localRotation = Quaternion.Euler(Vector3.zero);

            entry.GetComponent<PlayerInList>().Initialize(p.NickName, p.ActorNumber);

            playerListEntries.Add(p.ActorNumber, entry);

            if(!PhotonNetwork.OfflineMode)
                GetComponent<GodScript>().accionButtonToList();
        }
    }

    //Salida de sala
    public void OnLeaveGameButtonClicked()
    {
        Debug.Log("Saliendo de la sala" + idSala.GetComponent<Text>().text);
        if (PhotonNetwork.LeaveRoom())
            Debug.Log("El usuario "+ PlayerPrefs.GetString("characterPlayerName", "") + " ha abandonado la sala");
        else
            Debug.LogError("Error inesperado cuando " + PlayerPrefs.GetString("characterPlayerName", "") + " ha intentado abandonar la sala");
    }

    //Busqueda de salas


    //Administración de sala

    /**
     * Iniciar partida*/
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
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

    public void offlineMode(bool isOffline)
    {
        PhotonNetwork.OfflineMode = isOffline;
    }
}
