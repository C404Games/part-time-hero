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
        hints.Add("Los platos sencillos se hacen más rapido");
        hints.Add("Intenta quitar objetos a tus rivales");
        hints.Add("No te pierdas las últimas noticias, sigue a @C404Games en Twitter");
        hints.Add("A Karmelin le gusta coleccionar monedas antiguas");
        hints.Add("Tario aprendió a cocinar gracias a su padre");
        hints.Add("Overcooked está pasado de moda");
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
