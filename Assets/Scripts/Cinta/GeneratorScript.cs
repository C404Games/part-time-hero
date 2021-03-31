using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    public double tiempoAparicion;
    public GameObject prefab;

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
        GameObject newProduct = Instantiate(prefab, transform.position, Quaternion.identity);
        ProductInstance instance = newProduct.GetComponent<ProductInstance>();
        instance.id = ProductManager.rawProducts[idx].id;
        
    }
}
