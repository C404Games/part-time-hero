using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelUnlocker : MonoBehaviour
{

    public GameObject[] levelButtons;

    void Start()
    {
        int levelUnlocked = PlayerPrefs.GetInt("LEVEL_UNLOCKED", 0);
        for(int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].SetActive(i <= levelUnlocked);
        }
    }

}
