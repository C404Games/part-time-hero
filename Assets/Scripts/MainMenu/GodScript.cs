using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodScript : MonoBehaviour
{
    public float speed = 1;

    public GameObject mainCamera;
    public Transform point1;
    public Transform point2;

    [SerializeField] 
    private GameObject[] subMenus;

    private GameObject[] meshModels;

    private Vector3 point;

    private int playerCoins;

    private void Start()
    {
        point = mainCamera.transform.position;
        meshModels = GameObject.FindGameObjectsWithTag("Models");
    }

    private void Update()
    {
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, point, Time.deltaTime * speed);
    }

    public void accionButton(int position)
    {
        foreach (GameObject subMenu in subMenus)
        {
            subMenu.SetActive(false);
        }

        subMenus[position].SetActive(true);

        point = point2.position;

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
        foreach (GameObject model in meshModels)
        {
            model.GetComponent<MeshFilter>().mesh = Instantiate(mesh);
        }
    }
}
