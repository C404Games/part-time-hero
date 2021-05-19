using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class universalParameters : MonoBehaviour
{
    private static string _sceneToLoad;

    private static int model;
    private static Texture myTexture;

    private static float _sound;
    private static float _music;

    public string getSceneToLoad()
    {
        return _sceneToLoad;
    }

    public void setSceneToLoad(string sceneToLoad)
    {
        _sceneToLoad = sceneToLoad;
    }
    public int getModel()
    {
        return model;
    }

    public void setModel(int m)
    {
        model = m;
    }
    public Texture getMyTexture()
    {
        return myTexture;
    }

    public void setMyTexture(Texture texture)
    {
        myTexture = texture;
    }

    public float getSound()
    {
        return _sound;
    }

    public void setSound(float sound)
    {
        _sound = sound;
    }

    public float getMusic()
    {
        return _music;
    }

    public void setMusic(float music)
    {
        _music = music;
    }

}
