using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buyCharacter : MonoBehaviour
{
    public int price;
    public Text text;

    private GodScript god;

    private void Start()
    {
        god = GameObject.FindGameObjectWithTag("God").GetComponent<GodScript>();
        text.text = price.ToString();
    }

    public void buy()
    {
        if (god.getCoins() >= price)
        {
            god.removeCoins(price);
            transform.parent.gameObject.GetComponent<Button>().interactable = true;
            Destroy(this.gameObject);
        }
        else
            Debug.Log("No tiene suficientes monedas para comprar");
    }
}
