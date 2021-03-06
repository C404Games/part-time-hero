using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    public string _scene;
    public Animator menuAnimator;

    public GameObject loadPanel;

    private IEnumerator fadePhotonCorroutine;
    private IEnumerator fadeNonPhotonCorroutine;


    private void Start()
    {
        fadePhotonCorroutine = fadeOutPhotonScene(2.0f);
        fadeNonPhotonCorroutine = fadeOutScene(10.0f);
        if (string.IsNullOrEmpty(_scene))
            _scene = GetComponent<universalParameters>().getSceneToLoad();
    }

    public void changeToScene()
    {
        loadNextScene();
    }

    public void changeToSpecificScene(string scene)
    {
        _scene = scene;
        loadNextScene();
    }

    public void changeToScenePhoton()
    {
        /*
        if (menuAnimator == null)
        {
            PhotonNetwork.LoadLevel(_scene);
            return;
        }
        */

        if(loadPanel != null)
            loadPanel.SetActive(true);

        // Ñapa fea, pero nos vale. Se desbloquea el primer nivel tras el prólogo
        /*
        if(_scene == "Prologue")
            PlayerPrefs.SetInt("LEVEL_UNLOCKED", 1);
        else if(_scene == "Episode2")
            PlayerPrefs.SetInt("LEVEL_UNLOCKED", 2);
        else if (_scene == "Tabern - Level 1")
            PlayerPrefs.SetInt("LEVEL_UNLOCKED", 3);
        else if (_scene == "Episode3")
            PlayerPrefs.SetInt("LEVEL_UNLOCKED", 4);
            */
       

        string scene = GameObject.Find("GodObject").GetComponent<universalParameters>().getSceneToLoad();//GetComponent<universalParameters>().getSceneToLoad();
        switch (scene)
        {
            case "Tabern - Level 1":
                PlayerPrefs.SetInt("Scenary", 1);
                break;
            case "Smithy - Level 1":
                PlayerPrefs.SetInt("Scenary", 2);
                break;
        }
        menuAnimator.SetTrigger("fadeIn");
        StartCoroutine(fadePhotonCorroutine);
    }

    public IEnumerator fadeOutScene(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(_scene);
    }

    public IEnumerator fadeOutPhotonScene(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.LoadLevel(_scene);
    }

    private void loadNextScene()
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
                    }
                    else
                    {
                        PlayerPrefs.SetInt("tutorialActive", 0);
                    }
                    PlayerPrefs.SetInt("Scenary", 1);
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
                    PlayerPrefs.SetInt("Scenary", 2);
                    break;
                }
        }
        menuAnimator.SetTrigger("fadeIn");
        StartCoroutine(fadeNonPhotonCorroutine);
    }
}
