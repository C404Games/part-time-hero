﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    //List<GameObject> listaElementos;
    public double tiempoAparicion;

    private double tiempo = 0;

    private List<int> cumulativeProbabilities;

    // Start is called before the first frame update
    void Start()
    {
        cumulativeProbabilities = new List<int>();
        for(int i = 0; i < ProductManager.rawProducts.Count; i++)
            cumulativeProbabilities.Add(1);

        actualizarValores();
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;
        if(tiempo >= tiempoAparicion)
        {
            List<int> uniformList = new List<int>();

            for(int i = 0; i < ProductManager.rawProducts.Count; i++)
            {
                uniformList.AddRange(System.Linq.Enumerable.Repeat(ProductManager.rawProducts[i].id, cumulativeProbabilities[i]));
            }

            int num = Random.Range(0, uniformList.Count);

            generateProduct(num);
            //Instantiate(listaUniforme[num], Vector3.zero, Quaternion.identity);

            tiempo = 0;
        }
    }

    private void actualizarValores()
    {

        for (int i = 0; i < cumulativeProbabilities.Count; i++)
        {
            cumulativeProbabilities[i] = 1;
        }
    }

    private void generateProduct(int idx)
    {
        GameObject newProduct = new GameObject();
        newProduct.transform.position = transform.position;
        ProductInstance instance = newProduct.AddComponent<ProductInstance>();
        instance.id = ProductManager.rawProducts[idx].id;
        
    }
}
