using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioController : MonoBehaviour
{
    public enum type
    {
        Music,
        Sound
    }

    public type Tipo = type.Music;

    private universalParameters uP;

    // Start is called before the first frame update
    void Start()
    {
        uP = new universalParameters();
    }

    // Update is called once per frame
    void Update()
    {
        switch (Tipo)
        {
            case type.Music:
                GetComponent<AudioSource>().volume = uP.getMusic();
                break;
            case type.Sound:
                GetComponent<AudioSource>().volume = uP.getSound();
                break;
            default:
                break;
        }
    }
}
