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

    [SerializeField] 
    private GameObject[] subMenus;

    private GameObject[] meshModels;

    private Vector3 point;

    private int playerCoins = 0;

    private int lastPosition = 0;

    private void Start()
    {
        point = mainCamera.transform.position;
    }

    private void Update()
    {
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, point, Time.deltaTime * speed);
    }

    public void accionButton(int position)
    {
        lastPosition = position;

        foreach (GameObject subMenu in subMenus)
        {
            subMenu.SetActive(false);
        }

        subMenus[position].SetActive(true);

        point = point2.position;

    }

    public void accionButtonToList()
    {
        point = point3.position;
    }

    public void volverButtonFromList()
    {
        point = lastPosition == 1 ? point2.position : point1.position;
    }

    public void volverButton()
    {
        point = point1.position;
    }

    public void addCoins(int coins)
    {
        playerCoins += coins;
    }

    public bool removeCoins(int coins)
    {
        if (coins < playerCoins)
            return false;

        playerCoins -= coins;
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

    public void changeModels(Mesh mesh)
    {
        meshModels = GameObject.FindGameObjectsWithTag("Models");
        
        foreach (GameObject model in meshModels)
        {
            model.GetComponent<MeshFilter>().mesh = Instantiate(mesh);
        }
    }
}
