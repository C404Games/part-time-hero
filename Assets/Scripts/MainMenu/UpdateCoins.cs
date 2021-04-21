using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCoins : MonoBehaviour
{
    private GodScript god;
    // Start is called before the first frame update
    void Start()
    {
        god = GameObject.FindGameObjectWithTag("God").GetComponent<GodScript>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = god.getCoins().ToString();
    }
}
