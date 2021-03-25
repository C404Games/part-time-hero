using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    public List<GameObject> listaElementos;
    public double tiempoAparicion;

    private double tiempo = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject p in listaElementos)
        {
            p.GetComponent<Product>().probabilidadAcumulada = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;
        if(tiempo >= tiempoAparicion)
        {
            List<GameObject> listaUniforme = new List<GameObject>();

            //actualizar valores

            foreach(GameObject p in listaElementos)
            {
                for(int i = 0; i< p.GetComponent<Product>().probabilidadAcumulada; i++)
                {
                    listaUniforme.Add(p);
                }
            }

            int num = Random.Range(0, listaUniforme.Count);
            Instantiate(listaUniforme[num], Vector3.zero, Quaternion.identity);

            tiempo = 0;
        }
    }
}
