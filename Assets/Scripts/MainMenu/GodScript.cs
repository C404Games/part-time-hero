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

    private Vector3 point;

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
}
