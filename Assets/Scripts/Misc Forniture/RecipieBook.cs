using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipieBook : MonoBehaviour
{

    public GameObject recipieCanvas;
    public Image recipieImage;

    // Start is called before the first frame update
    void Start()
    {

        recipieCanvas.SetActive(false);

        if (recipieImage.sprite == null)
        {
            int level = FindObjectOfType<MatchManager>().getLevel();

            switch (level)
            {
                case 0:
                    recipieImage.sprite = Resources.Load<Sprite>("Images/Recipies/tabern");
                    break;
                default:
                    recipieImage.sprite = Resources.Load<Sprite>("Images/Recipies/tabern");
                    break;
            }
        }
    }

    public bool isOpen()
    {
        return recipieCanvas.activeSelf;
    }

    public void openBook()
    {
        recipieCanvas.SetActive(true);
    }

    public void closeBook()
    {
        recipieCanvas.SetActive(false);
    }
}
