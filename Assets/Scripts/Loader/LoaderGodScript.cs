using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoaderGodScript : MonoBehaviour
{
    [SerializeField]
    public GameObject[] otherPlayers;

    public int numberOtherPlayers;

    public GameObject swordIndicator;

    // Start is called before the first frame update
    void Start()
    {

        foreach (GameObject item in otherPlayers)
        {
            item.SetActive(false);
        }

        //numberOtherPlayers = otherPlayers.Length < numberOtherPlayers ? otherPlayers.Length : numberOtherPlayers;

        numberOtherPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("Cargando sala con " + numberOtherPlayers + " jugadores");
        int count = 1;
        for (int i = 0; i < numberOtherPlayers; i++)
        {
            otherPlayers[i].SetActive(true);
            if (PhotonNetwork.CurrentRoom.GetPlayer(i + 1).NickName != PhotonNetwork.NickName)
            {
                otherPlayers[count].GetComponent<Text>().text = PhotonNetwork.CurrentRoom.GetPlayer(i + 1).NickName;
                count++;
            }
        }
    }

    private float time = 0;
    private bool posible = true;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        //jugador 1 ("yo")
        if(time >= 5)
        {
            otherPlayers[0].GetComponent<SpriteAnimator>().stopSecuence();
            swordIndicator.GetComponent<CompleteScript>().visible = true;
            swordIndicator.GetComponent<CompleteScript>().complete = true;
        }

        if (time >= 2 || numberOtherPlayers >= 2)
            otherPlayers[1].transform.GetChild(0).gameObject.GetComponent<CompleteScript>().complete = true;

        if (time >= 3 || numberOtherPlayers >= 3)
            otherPlayers[2].transform.GetChild(0).gameObject.GetComponent<CompleteScript>().complete = true;

        if (time >= 1.5f || numberOtherPlayers >= 4)
            otherPlayers[3].transform.GetChild(0).gameObject.GetComponent<CompleteScript>().complete = true;

        if (time >= 6 && PhotonNetwork.IsMasterClient && posible)
        {
            GetComponent<loadScene>().changeToScenePhoton();
            posible = false;
        }


    }
}
