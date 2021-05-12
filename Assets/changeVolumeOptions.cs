using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeVolumeOptions : MonoBehaviour
{
    public Slider efectoSonido;
    public Slider musica;
    public void changeVolume()
    {
        GameObject.Find("GodObject").gameObject.GetComponent<universalParameters>().setSound(efectoSonido.value);
        GameObject.Find("GodObject").gameObject.GetComponent<universalParameters>().setSound(musica.value);
    }
}
