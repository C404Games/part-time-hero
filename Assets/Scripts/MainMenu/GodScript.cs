using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodScript : MonoBehaviour
{
    public float speed = 1;

    public GameObject mainCamera;
    public Transform point1;
    public Transform point2;
    public Transform point3;
    
    [HideInInspector]
    public Vector3 point;
    
    [HideInInspector]
    public int position = 0;
    
    [SerializeField] 
    private GameObject[] subMenus;

    public GameObject[] modelList1;
    public GameObject[] modelList2;
    
    
    private int playerCoins = 0;

    private int lastPosition = 0;

    private universalParameters uP;

   

    private void Start()
    {
        point = mainCamera.transform.position;
        uP = GetComponent<universalParameters>();
        for(int i = 0; i < modelList1.Length; i++)
        {
            if (i == uP.getModel())
            {
                modelList1[i].SetActive(true);
                modelList2[i].SetActive(true);
            }
            else
            {
                modelList1[i].SetActive(false);
                modelList2[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, point, Time.deltaTime * speed);
    }

    public void accionButton(int position)
    {
        this.position = position;

        foreach (GameObject subMenu in subMenus)
        {
            subMenu.SetActive(false);
        }

        subMenus[position].SetActive(true);

        if (position == 1 || position == 3) {
            GetComponent<PhotonController>().OnLoginButtonClicked();
        }
        else
        {
            lastPosition = position;

            

            point = point2.position;
        }
    }

    public void accionButtonToList()
    {
        point = point3.position;
    }

    public void volverButtonFromList()
    {
        if (position == 3)
            GetComponent<PhotonController>().onDisconnectButtonclicked();
        point = (position != 3) ? point2.position : point1.position;
        position = 0;
    }

    public void volverButton()
    {
        position = 0;
        point = point1.position;
    }

    public void addCoins(int coins)
    {
        playerCoins += coins;
        //PlayerPrefs.SetInt("characterMoneyName", PlayerPrefs.GetInt("characterMoneyName", 0) + coins);
    }

    public bool removeCoins(int coins)
    {
        if (coins < playerCoins)
        {
            //PlayerPrefs.SetInt("characterMoneyName", 0);
            playerCoins -= coins;
            return false;
        }

        playerCoins -= coins;
        //PlayerPrefs.SetInt("characterMoneyName", PlayerPrefs.GetInt("characterMoneyName", 0) - coins);
        return true;
    }

    public int getCoins()
    {
        return playerCoins;
    }

    public void setCoins(int coins)
    {
        playerCoins = coins;
    }

    public void changeModels(int idx)
    {
        modelList1[uP.getModel()].gameObject.SetActive(false);
        modelList2[uP.getModel()].gameObject.SetActive(false);

        modelList1[idx].gameObject.SetActive(true);
        modelList2[idx].gameObject.SetActive(true);

        uP.setModel(idx);
    }
    public void changeSkin(Texture texture)
    {
        /*
        meshModels = GameObject.FindGameObjectsWithTag("Models");

        foreach (GameObject model in meshModels)
        {
            model.GetComponent<MeshRenderer>().material.mainTexture = texture;
        }

        uP.setMyTexture(texture);
        */
    }
}
