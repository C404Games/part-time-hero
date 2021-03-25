using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatcherScript : MonoBehaviour
{
    private List<GameObject> listaTargets;
    private List<GameObject> listaObjetos;

    public GameObject objetoMasCercano()
    {
        double minDistance = Mathf.Infinity;
        GameObject minDistanceObject = null;

        foreach (GameObject gameObject in listaObjetos)
        {
            if(Vector3.Distance(gameObject.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(gameObject.transform.position, transform.position);
                minDistanceObject = gameObject;
            }
        }

        return minDistanceObject;
    }

    public GameObject targetMasCercano()
    {
        double minDistance = Mathf.Infinity;
        GameObject minDistanceTarget = null;

        foreach (GameObject gameObject in listaTargets)
        {
            if (Vector3.Distance(gameObject.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(gameObject.transform.position, transform.position);
                minDistanceTarget = gameObject;
            }
        }

        return minDistanceTarget;
    }

    public void elimiarObjeto(GameObject objeto)
    {
        listaObjetos.Remove(objeto);
    }

    private void Start()
    {
        listaObjetos = new List<GameObject>();
        listaTargets = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
            listaTargets.Add(other.gameObject);
        else if (other.tag == "Item")
            listaObjetos.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Target")
            listaTargets.Remove(other.gameObject);
        else if (other.tag == "Item")
            listaObjetos.Remove(other.gameObject);
    }
}
