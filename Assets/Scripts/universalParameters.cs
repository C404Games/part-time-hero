using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class universalParameters : MonoBehaviour
{
    private static string _sceneToLoad;

    public string getSceneToLoad()
    {
        return _sceneToLoad;
    }

    public void setSceneToLoad(string sceneToLoad)
    {
        _sceneToLoad = sceneToLoad;
    }
}
