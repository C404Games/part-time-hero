using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    public string _scene;
    public Animator menuAnimator;
    private IEnumerator fadeCorroutine;

    private void Awake()
    {
        fadeCorroutine = fadeOutScene(1.0f);
    }


    private void Start()
    {
        if (string.IsNullOrEmpty(_scene))
            _scene = GetComponent<universalParameters>().getSceneToLoad();
    }

    public void changeToScene()
    {
        switch (_scene)
        {

            case ("HistoryFirstLevel"):
                {
                    PlayerPrefs.SetInt("tutorialActive", 1);
                    break;
                }
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
                    PlayerPrefs.SetString("map", "taberna");
                    break;
                }
            case ("Smithy - Level 1"):
                {
                    int level = PlayerPrefs.GetInt("storyLevelAvailable", -1);
                    if (level <= 2)
                    {
                        PlayerPrefs.SetInt("tutorialActive", 1);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("tutorialActive", 0);
                    }
                    PlayerPrefs.SetString("map", "herreria");
                    break;
                }
        }
        menuAnimator.SetTrigger("fadeIn");
        StartCoroutine(fadeCorroutine);
    }

    public void changeToSpecificScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void changeToScenePhoton()
    {
        PhotonNetwork.LoadLevel(_scene);
    }

    public IEnumerator fadeOutScene(float time)
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(_scene);
    }
}
