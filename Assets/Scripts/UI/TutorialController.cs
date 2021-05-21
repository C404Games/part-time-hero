using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Polyglot;

public class TutorialController : MonoBehaviour
{

    public LocalizedText key;

    public GameObject arrowPrefab;

    public Animator animator;

    private TutorialStep[] steps;

    private MatchManager matchManager;

    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        matchManager = FindObjectOfType<MatchManager>();

        if (!PhotonNetwork.OfflineMode || PlayerPrefs.GetInt("scenary") == 2)
        {
            animator.SetBool("tutorialDone", true);
            return;
        }

        //matchManager.pauseMatch();

        steps = transform.GetComponentsInChildren<TutorialStep>();
        if (steps.Length == 0)
        {
            animator.SetBool("tutorialDone", true);
            gameObject.SetActive(false);
            return;
        }

        foreach (TutorialStep step in steps)
        {
            step.gameObject.SetActive(false);
            step.arrowPrefab = arrowPrefab;
        }

        steps[index].gameObject.SetActive(true);
        key.Key = steps[index].text;
    }

    public void onClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (index >= steps.Length - 1)
            {
                animator.SetBool("tutorialDone", true);
                Destroy(gameObject);
            }
            else
            {
                steps[index++].gameObject.SetActive(false);
                steps[index].gameObject.SetActive(true);
                key.Key = steps[index].text;
            }
        }

    }
}
