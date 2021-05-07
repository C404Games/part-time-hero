using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    public string _scene;

    private void Start()
    {
        if (string.IsNullOrEmpty(_scene))
            _scene = GetComponent<universalParameters>().getSceneToLoad();
    }

    public void changeToScene()
    {
        switch (_scene)
        {
            case ("Tabern - Level 1"):
                {
                    int level = PlayerPrefs.GetInt("storyLevelAvailable", -1);
                    if (level <= 1)
                    {
                        PlayerPrefs.SetInt("tutorialActive", 1);
                    } else
                    {
                        PlayerPrefs.SetInt("tutorialActive", 0);
                    }
                    break;
                }
            case ("HistoryFirstLevel"):
                {
                    PlayerPrefs.SetInt("tutorialActive", 1);
                    break;
                }
        }
        SceneManager.LoadScene(_scene);
    }

    public void changeToSpecificScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
