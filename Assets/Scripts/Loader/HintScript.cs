using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintScript : MonoBehaviour
{

    private List<string> hints;
    private int hint;

    // Start is called before the first frame update
    void Start()
    {
        //Cargar los elementos de un archivo, para intentar evitar hacerlo mediante hardcode
        // hints = cargarElementos();
        hints = new List<string>();
        hints.Add("Pista 1");
        hints.Add("Pista 2");
        hints.Add("Pista no tan secreta");
        hints.Add("El rino hace buenas espadas");
        hints.Add("La cerveza de cereza no le ha molado al profe");
        hints.Add("Soy un estegosaurio");
        hints.Add("I like trains");

        hint = Random.Range(0, hints.Count);
        GetComponent<Text>().text = hints[hint];
    }

    public void newHint()
    {
        int nHint;

        //Comprobamos que el nuevo mensaje no es igual al anterior
        do
            nHint = Random.Range(0, hints.Count - 1);
        while (nHint == hint);

        hint = nHint;
        GetComponent<Text>().text = hints[nHint];
    }
}
