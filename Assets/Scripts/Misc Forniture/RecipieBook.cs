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
            int level = PlayerPrefs.GetInt("Scenary", 1);

            switch (level)
            {
                case 1:
                    recipieImage.sprite = Resources.Load<Sprite>("Images/Recipies/tabern");
                    break;
                case 2:
                    recipieImage.sprite = Resources.Load<Sprite>("Images/Recipies/smithy");
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
