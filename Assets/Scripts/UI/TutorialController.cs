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

        if (!PhotonNetwork.OfflineMode)
        {
            animator.SetBool("tutorialDone", true);
            return;
        }

        matchManager.pauseMatch();

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

        if (index == 0)
        {
            switch (PlayerPrefs.GetString("language", "Spanish"))
            {
                case "Spanish":
                    {
                        if (PlayerPrefs.GetInt("Scenary") == 1)
                        {
                            key.Key = "¡Bienvenido a la Taberna de Tario!";
                        } else if (PlayerPrefs.GetInt("Scenary") == 2)
                        {
                            key.Key = "¡Bienvenido a la Herrería de Rairon!";
                        }
                        break;
                    }
                case "English":
                    {
                        if (PlayerPrefs.GetInt("Scenary") == 1)
                        {
                            key.Key = "Welcome to Tario's tavern！";
                        }
                        else if (PlayerPrefs.GetInt("Scenary") == 2)
                        {
                            key.Key = "Welcome to Rairon's smithy！";
                        }
                        break;
                    }
                case "Simplified Chinese":
                    {
                        if (PlayerPrefs.GetInt("Scenary") == 1)
                        {
                            key.Key = "歡迎來到塔里奧酒館！";
                        }
                        else if (PlayerPrefs.GetInt("Scenary") == 2)
                        {
                            key.Key = "歡迎來到Rairon Smithy！";
                        }
                        break;
                    }
            }
        }
        else if (index == 1)
        {
            switch (PlayerPrefs.GetString("language", "Spanish"))
            {
                case "Spanish":
                    {
                        key.Key = "Recoge los producto que necesitas de las cintas";
                        break;
                    }
                case "English":
                    {
                        key.Key = "Collect the products you need from the tapes";
                        break;
                    }
                case "Simplified Chinese":
                    {
                        key.Key = "從錄像帶中收集您需要的產品";
                        break;
                    }
            }
        }
        else if (index == 2)
        {
            switch (PlayerPrefs.GetString("language", "Spanish"))
            {
                case "Spanish":
                    {
                        key.Key = "Puedes pulsar el libro para ver el recetario";
                        break;
                    }
                case "English":
                    {
                        key.Key = "You can click on the book to see the recipe book";
                        break;
                    }
                case "Simplified Chinese":
                    {
                        key.Key = "您可以單擊書以查看食譜書";
                        break;
                    }
            }
        }
        else if (index == 3)
        {
            switch (PlayerPrefs.GetString("language", "Spanish"))
            {
                case "Spanish":
                    {
                        key.Key = "Lleva las recetas terminadas al punto de entrega";
                        break;
                    }
                case "English":
                    {
                        key.Key = "Take the finished recipes to the delivery point";
                        break;
                    }
                case "Simplified Chinese":
                    {
                        key.Key = "將完成的食譜帶到交貨地點";
                        break;
                    }
            }
        }
        //key.Key = steps[index].text;
    }

    public void onClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (index >= steps.Length-1)
            {
                animator.SetBool("tutorialDone", true);
                gameObject.SetActive(false);
            }
            else
            {
                steps[index++].gameObject.SetActive(false);
                steps[index].gameObject.SetActive(true);

                if (index == 0)
                {
                    switch (PlayerPrefs.GetString("language", "Spanish"))
                    {
                        case "Spanish":
                            {
                                key.Key = "¡Bienvenido a la Taberna de Tario!";
                                break;
                            }
                        case "English":
                            {
                                key.Key = "Welcome to Tario's tavern！";
                                break;
                            }
                        case "Simplified Chinese":
                            {
                                key.Key = "歡迎來到塔里奧酒館！";
                                break;
                            }
                    }
                }
                else if (index == 1)
                {
                    switch (PlayerPrefs.GetString("language", "Spanish"))
                    {
                        case "Spanish":
                            {
                                key.Key = "Recoge los producto que necesitas de las cintas";
                                break;
                            }
                        case "English":
                            {
                                key.Key = "Collect the products you need from the tapes";
                                break;
                            }
                        case "Simplified Chinese":
                            {
                                key.Key = "從錄像帶中收集您需要的產品";
                                break;
                            }
                    }
                }
                else if (index == 2)
                {
                    switch (PlayerPrefs.GetString("language", "Spanish"))
                    {
                        case "Spanish":
                            {
                                key.Key = "Puedes pulsar el libro para ver el recetario";
                                break;
                            }
                        case "English":
                            {
                                key.Key = "You can click on the book to see the recipe book";
                                break;
                            }
                        case "Simplified Chinese":
                            {
                                key.Key = "您可以單擊書以查看食譜書";
                                break;
                            }
                    }
                }
                else if (index == 3)
                {
                    switch (PlayerPrefs.GetString("language", "Spanish"))
                    {
                        case "Spanish":
                            {
                                key.Key = "Lleva las recetas terminadas al punto de entrega";
                                break;
                            }
                        case "English":
                            {
                                key.Key = "Take the finished recipes to the delivery point";
                                break;
                            }
                        case "Simplified Chinese":
                            {
                                key.Key = "將完成的食譜帶到交貨地點";
                                break;
                            }
                    }
                }
                //key.Key = steps[index].text;
            }
        }

    }
}
